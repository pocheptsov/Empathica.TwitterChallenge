using System.Collections.Generic;
using System.Threading;
using Empathica.TwitterChallenge.Db;

namespace Empathica.TwitterChallenge.Wires.Resources.Strategy
{
    public class StaticCacheStrategy : ICacheStrategy
    {
        private readonly IRepository _repository;
        //consider StringDictionary
        private static Dictionary<string, string> _resources;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        public StaticCacheStrategy(IRepository repository)
        {
            _repository = repository;
        }

        public void Init()
        {
            if (!_isInitialized)
            {
                LazyInitializer.EnsureInitialized(ref _resources,
                                                  ref _isInitialized,
                                                  ref _initializerLock,
                                                  () => _repository.GetResources());
            }
        }

        public string this[string key]
        {
            get { return _resources[key]; }
        }
    }
}