using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Empathica.TwitterChallenge.Db.Domain;
using Newtonsoft.Json;

namespace Empathica.TwitterChallenge.Db
{
    public interface IDbContext : IDisposable
    {
        IDictionary<int, TwitterStatus> Statuses { get; }
        IDictionary<string, StringResource> Resources { get; }
        void SaveChanges();
    }

    /// <summary>
    /// Fake database implementation.
    /// Store data in serialized mode in json files.
    /// </summary>
    public class DbContext : IDbContext
    {
        //simple db in-memory store
        private const string ResourcesFilename = "resources.json";
        private const string StatusesFilename = "statuses.json";

        //concurrent db engine
        private static readonly ConcurrentDictionary<int, TwitterStatus> _statuses =
            new ConcurrentDictionary<int, TwitterStatus>();

        private static readonly ConcurrentDictionary<string, StringResource> _resources =
            new ConcurrentDictionary<string, StringResource>();

        protected static string BaseFolder { get; set; }

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
            var serializer = new JsonSerializer();
            using (StreamWriter file = File.CreateText(Path.Combine(BaseFolder, "statuses.json")))
            {
                serializer.Serialize(file, _statuses.Values);
            }

            /*using (StreamWriter file = File.CreateText(Path.Combine(BaseFolder, "resources.json")))
            {
                serializer.Serialize(file, _resources.Values);
            }*/
        }

        public void Dispose()
        {
            _resources.Clear();
            _statuses.Clear();
        }

        public static void Init(string baseFolder)
        {
            BaseFolder = baseFolder;

            //InitData();
            var serializer = new JsonSerializer();
            using (StreamReader file = File.OpenText(Path.Combine(BaseFolder, StatusesFilename)))
            {
                foreach (
                    var twitterStatus in
                        (List<TwitterStatus>) serializer.Deserialize(file, typeof (List<TwitterStatus>)))
                {
                    _statuses.TryAdd(twitterStatus.Id, twitterStatus);
                }
            }

            using (StreamReader file = File.OpenText(Path.Combine(BaseFolder, ResourcesFilename)))
            {
                foreach (
                    var stringResource in
                        (List<StringResource>)serializer.Deserialize(file, typeof(List<StringResource>)))
                {
                    _resources.TryAdd(stringResource.Key, stringResource);
                }
            }
        }
    }
}