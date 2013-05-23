using System;
using System.Web.Mvc;
using Empathica.TwitterChallenge.Db;
using Empathica.TwitterChallenge.Tests.Mocks;
using Empathica.TwitterChallenge.Web.Controllers;
using Empathica.TwitterChallenge.Web.Extensions;
using Empathica.TwitterChallenge.Wires.Config;
using Spring.Rest.Client.Testing;
using Spring.Social.Twitter.Api.Impl;

namespace Empathica.TwitterChallenge.Tests
{
    public class SpecificationsBase
    {
        protected Func<IDbContext, Container> containerFactory;
        protected Func<IDbContext> dbContextFactory;
        protected MockRestServiceServer mockServer;
        protected Func<ISessionAdapter> sessionFactory;
        protected TwitterTemplate twitter;

        public SpecificationsBase()
        {
            twitter = new TwitterTemplate("API_KEY", "API_SECRET", "ACCESS_TOKEN", "ACCESS_TOKEN_SECRET");
            mockServer = MockRestServiceServer.CreateServer(twitter.RestTemplate);
            dbContextFactory = () => new TestDbContext();
            containerFactory = dbContext => new Container
                                                {
                                                    RepositoryFactory =
                                                        () => new SimpleRepository(dbContext ?? dbContextFactory()),
                                                };
            sessionFactory = () => new TestSessionAdapter();
        }

        protected TwitterController GetTwitterController(Container container = null, ISessionAdapter session = null)
        {
            return new TwitterController(container, session)
                       {
                           ControllerContext =
                               new ControllerContext()
                       };
        }
    }
}