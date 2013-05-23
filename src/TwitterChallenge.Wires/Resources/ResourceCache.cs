using Empathica.TwitterChallenge.Wires.Resources.Strategy;

namespace Empathica.TwitterChallenge.Wires.Resources
{
    public class ResourceCache : IResourceCache
    {
        /// <summary>
        /// Statis initialization cashing strategy, must be called before using instance of class 
        /// </summary>
        public static void Init(StaticCacheStrategy cacheStrategy)
        {
            //todo: check that init only once
            CacheStrategy = cacheStrategy;
            cacheStrategy.Init();

            //HttpContext.Current.Application["key"]
//            HttpRuntime.Cache.Insert(
//                /* key */                "key",
//                /* value */              suppliers,
//                /* dependencies */       null,
//                /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
//                /* slidingExpiration */  Cache.NoSlidingExpiration,
//                /* priority */           CacheItemPriority.NotRemovable,
//                /* onRemoveCallback */   null);
        }

        private static ICacheStrategy CacheStrategy { get; set; }

        public string this[string key]
        {
            get { return CacheStrategy[key]; }
        }
    }
}