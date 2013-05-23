using System.Collections.Concurrent;
using System.Collections.Generic;
using Empathica.TwitterChallenge.Db;
using Empathica.TwitterChallenge.Db.Domain;
using Empathica.TwitterChallenge.Wires.Resources;

namespace Empathica.TwitterChallenge.Tests.Mocks
{
    public class TestDbContext : IDbContext
    {
        protected const string status1 = "test status";
        protected const string status2 = "test status";
        protected const string status3 = "test status";
        protected const string status4 = "test status";
        protected const string status5 = "test status";

        private readonly ConcurrentDictionary<string, StringResource> _resources =
            new ConcurrentDictionary<string, StringResource>();

        private readonly ConcurrentDictionary<int, TwitterStatus> _statuses =
            new ConcurrentDictionary<int, TwitterStatus>();


        protected Dictionary<string, string> testResources =
            new Dictionary<string, string>
                {
                    {Rk.Hello, "Hello world"},
                    {Rk.BtnDoneText, "Done"},
                    {Rk.BtnBackText, "Back"}
                };

        protected string[] testStatuses = new[] {status1, status2, status3, status4, status5};

        public TestDbContext()
        {
            for (int ind = 0; ind < testStatuses.Length; ind++)
            {
                Statuses.Add(ind + 1,
                             new TwitterStatus
                                 {
                                     Id = ind + 1,
                                     Text = testStatuses[ind]
                                 });
            }

            foreach (var testResource in testResources)
            {
                Resources.Add(testResource.Key,
                              new StringResource
                                  {
                                      Key = testResource.Key,
                                      Value = testResource.Value,
                                  });
            }
        }

        public IDictionary<int, TwitterStatus> Statuses
        {
            get { return _statuses; }
        }

        public IDictionary<string, StringResource> Resources
        {
            get { return _resources; }
        }

        public void SaveChanges()
        {
        }

        public void Dispose()
        {
            _resources.Clear();
            _statuses.Clear();
        }
    }
}