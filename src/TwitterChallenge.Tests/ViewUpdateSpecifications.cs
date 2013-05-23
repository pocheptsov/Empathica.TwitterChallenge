using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Empathica.TwitterChallenge.Db.Domain;
using Empathica.TwitterChallenge.Tests.Mocks;
using Empathica.TwitterChallenge.Web.Controllers;
using Empathica.TwitterChallenge.Web.ViewModels;
using Empathica.TwitterChallenge.Wires.Config;
using Simple.Testing.ClientFramework;

namespace Empathica.TwitterChallenge.Tests
{
    public class ViewUpdateSpecifications : SpecificationsBase
    {
        public Specification when_viewing_status_with_correct_id()
        {
            var testDbContext = new TestDbContext();
            Container container = containerFactory(testDbContext);
            KeyValuePair<int, TwitterStatus> testStatus = testDbContext.Statuses.First(_ => _.Key == 1);

            return new QuerySpecification<TwitterController, ViewResult>
                       {
                           On = () => GetTwitterController(container),
                           When = ctrl => ctrl.View(1) as ViewResult,
                           Expect =
                               {
                                   viewResult => viewResult.Model is TwitterStatus,
                                   viewResult => ((TwitterStatus) viewResult.Model).Text == testStatus.Value.Text,
                               },
                       };
        }
        
        public Specification when_viewing_status_with_incorrect_id()
        {
            var testDbContext = new TestDbContext();
            Container container = containerFactory(testDbContext);

            return new FailingSpecification<TwitterController, KeyNotFoundException>
                       {
                           On = () => GetTwitterController(container),
                           When = ctrl => ctrl.View(100),
                           Expect =
                               {
                                   ex => ex != null,
                               },
                       };
        }
    }
}