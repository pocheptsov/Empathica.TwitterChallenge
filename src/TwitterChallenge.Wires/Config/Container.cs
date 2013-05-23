using System;
using Empathica.TwitterChallenge.Db;
using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api;

namespace Empathica.TwitterChallenge.Wires.Config
{
    public sealed class Container : IDisposable
    {
        internal Setup Setup { get; set; }

        public Func<IOAuth1ServiceProvider<ITwitter>> TwitterServiceFactory { get; set; }

        public Func<IRepository> RepositoryFactory { get; set; }

        public void Dispose()
        {
        }
    }
}