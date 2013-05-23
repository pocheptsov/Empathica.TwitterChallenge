using System;
using System.Web;
using Empathica.TwitterChallenge.Db;
using Empathica.TwitterChallenge.Wires.Resources;
using Empathica.TwitterChallenge.Wires.Resources.Strategy;
using Spring.Social.Twitter.Connect;

namespace Empathica.TwitterChallenge.Wires.Config
{
    public sealed class Setup
    {
        /*static readonly string FunctionalRecorderQueueName = Conventions.FunctionalEventRecorderQueue;

        public static readonly EnvelopeStreamer Streamer = Contracts.CreateStreamer();
        public static readonly IDocumentStrategy ViewStrategy = new ViewStrategy();
        public static readonly IDocumentStrategy DocStrategy = new DocumentStrategy();

        public IStreamRoot Streaming;

        public Func<string, IQueueWriter> QueueWriterFactory;
        public Func<string, IQueueReader> QueueReaderFactory;
        public Func<string, IAppendOnlyStore> AppendOnlyStoreFactory;
        public Func<IDocumentStrategy, IDocumentStore> DocumentStoreFactory;*/

        public static readonly string TwitterConsumerKey = @"NUMFjfq3VAf8MdEgpJ3A";
        public static readonly string TwitterConsumerSecret = @"RRXOWzXRLsR2C1zOv1H1ZgH7mrnJSB2RbcCuSHnolqE";

        public static Func<HttpContextBase, IResourceCache> CacheFactory;
        public static Func<IRepository> RepositoryFactory;

        static Setup()
        {
            //order important
            var baseFolder = HttpContext.Current.Server.MapPath("~/data");
            DbContext.Init(baseFolder);
            RepositoryFactory = () => new SimpleRepository(new DbContext());
            ResourceCache.Init(new StaticCacheStrategy(RepositoryFactory()));

            CacheFactory = ctx => new ResourceCache();
        }

        public Container Build()
        {
            return new Container
            {
                Setup = this,
                TwitterServiceFactory = () => new TwitterServiceProvider(Setup.TwitterConsumerKey, Setup.TwitterConsumerSecret),
                RepositoryFactory = () => Setup.RepositoryFactory(),
            };
        }
    }
}