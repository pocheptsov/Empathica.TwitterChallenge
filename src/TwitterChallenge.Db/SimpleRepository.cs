using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Empathica.TwitterChallenge.Db.Domain;

namespace Empathica.TwitterChallenge.Db
{
    public interface IRepository : IDisposable
    {
        T GetById<T>(int id) where T : IEntity;
        Dictionary<string, string> GetResources();
        TwitterStatus AddStatus(TwitterStatus status);
        TwitterStatus GetStatusById(int id);
    }

    /// <summary>
    /// Very simplified repository pattern
    /// </summary>
    public class SimpleRepository : IRepository
    {
        private readonly IDbContext _dbContext;

        public SimpleRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public T GetById<T>(int id) where T : IEntity
        {
            dynamic collection = null;
            var type = typeof (T);
            if (type == typeof (TwitterStatus))
            {
                collection = _dbContext.Statuses;
            }
            /*else if (type == typeof(StringResource))
            {
                collection = DbContext.Resources;
            }*/

            if (collection == null)
            {
                throw new ArgumentException("Unsupported type for storing.");
            }

            return (T)collection[id];
        }

        public TwitterStatus AddStatus(TwitterStatus status)
        {
            int lastId = _dbContext.Statuses.DefaultIfEmpty().Max(_ => _.Key);

            //safe loop to process concurrent id
            //very hard code variant
            RetryUntilSuccessOrTimeout(() =>
                                           {
                                               lastId++;
                                               try
                                               {
                                                   status.Id = lastId;
                                                   _dbContext.Statuses.Add(lastId, status);
                                                   return true;
                                               }
                                               catch (Exception)
                                               {
                                                   //no need to log anything
                                                   return false;
                                               }
                                           },
                                       TimeSpan.FromMilliseconds(201),
                                       TimeSpan.FromMilliseconds(100));
            return status;
        }

        public TwitterStatus GetStatusById(int id)
        {
            return _dbContext.Statuses[id];
        }

        public Dictionary<string, string> GetResources()
        {
            return _dbContext.Resources.ToDictionary(_ => _.Key, _ => _.Value.Value);
        }

        public void Dispose()
        {
            _dbContext.SaveChanges();
        }

        private bool RetryUntilSuccessOrTimeout(Func<bool> task, TimeSpan timeout, TimeSpan pause)
        {
            if (pause.TotalMilliseconds < 0)
            {
                throw new ArgumentException("pause must be >= 0 milliseconds");
            }
            var stopwatch = Stopwatch.StartNew();
            do
            {
                if (task())
                {
                    return true;
                }
                Thread.Sleep((int)pause.TotalMilliseconds);
            }
            while (stopwatch.Elapsed < timeout);
            return false;
        }
    }
}