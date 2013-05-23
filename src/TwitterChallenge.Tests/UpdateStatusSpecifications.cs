using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Empathica.TwitterChallenge.Tests.Mocks;
using Empathica.TwitterChallenge.Web.Controllers;
using Empathica.TwitterChallenge.Web.Extensions;
using Empathica.TwitterChallenge.Web.ViewModels;
using Empathica.TwitterChallenge.Wires.Config;
using Moq;
using Simple.Testing.ClientFramework;
using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api;

namespace Empathica.TwitterChallenge.Tests
{
    public class UpdateStatusSpecifications : SpecificationsBase
    {
        public Specification when_updating_status_with_access_token()
        {
            const string statusText = "some test status";
            var statusViewModel = new StatusViewModel
                                      {
                                          Status = statusText
                                      };
            ISessionAdapter session = sessionFactory();
            var testDbContext = new TestDbContext();
            Container container = containerFactory(testDbContext);

            var mockTwitterProvider = MockTwitterProvider(statusText);

            container.TwitterServiceFactory = () => mockTwitterProvider.Object;

            int statusCount = testDbContext.Statuses.Count;

            return new ActionSpecification<TwitterController>
                       {
                           On = () =>
                                    {
                                        session.SetTwitterAccessToken(new OAuthToken("", ""));
                                        return GetTwitterController(container, session);
                                    },
                           When = ctrl => ctrl.UpdateStatus(statusViewModel),
                           Expect =
                               {
                                   ctrl => testDbContext.Statuses.Count == statusCount + 1,
                                   ctrl => testDbContext.Statuses.Last().Value.Text == statusText,
                               },
                       };
        }

        public Specification when_updating_status_with_no_access_token()
        {
            const string statusText = "some test status";
            var statusViewModel = new StatusViewModel
            {
                Status = statusText
            };
            ISessionAdapter session = sessionFactory();

            return new QuerySpecification<TwitterController, ActionResult>
            {
                On = () => GetTwitterController(session: session),
                When = ctrl => ctrl.UpdateStatus(statusViewModel),
                Expect =
                               {
                                   actionResult => session.GetTwitterNewStatus() == statusText,
                                   actionResult => actionResult is RedirectToRouteResult,
                                   actionResult =>
                                           ((RedirectToRouteResult)actionResult).RouteValues["action"] == "SignIn",
                               },
            };
        }

        const char c = ' ';

        public Specification when_updating_status_with_invalid_model()
        {
            string statusText = new string('*', 141);
            var statusViewModel = new StatusViewModel
            {
                Status = statusText
            };
            ISessionAdapter session = sessionFactory();
            var testDbContext = new TestDbContext();
            Container container = containerFactory(testDbContext);

            int maxId = testDbContext.Statuses.Max(_ => _.Key);

            return new QuerySpecification<TwitterController, ViewResult>
            {
                On = () =>
                         {
                             session.SetTwitterAccessToken(new OAuthToken("", ""));
                             return GetTwitterController(container, session);
                         },
                When = ctrl => ctrl.UpdateStatus(statusViewModel) as ViewResult,
                Expect =
                               {
                                   routeResult =>routeResult.ViewName == "Send",
                               },
            };
        }

        private static Mock<IOAuth1ServiceProvider<ITwitter>> MockTwitterProvider(string statusText)
        {
            //mock timeline operations
            var mockOperations = new Mock<ITimelineOperations>();
            var task = new Task<Tweet>(() => new Tweet {Text = statusText});
            mockOperations.Setup(_ => _.UpdateStatusAsync(statusText)).Returns(task);

            //mock twitter template
            var mockTwitter = new Mock<ITwitter>();
            mockTwitter.Setup(_ => _.TimelineOperations).Returns(mockOperations.Object);

            //mock twitter provider
            var mockTwitterProvider = new Mock<IOAuth1ServiceProvider<ITwitter>>();
            mockTwitterProvider.Setup(_ => _.GetApi("", "")).Returns(mockTwitter.Object);
            return mockTwitterProvider;
        }
    }
}