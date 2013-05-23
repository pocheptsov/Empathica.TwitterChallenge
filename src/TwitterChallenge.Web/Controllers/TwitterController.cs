using System;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using Empathica.TwitterChallenge.Db;
using Empathica.TwitterChallenge.Db.Domain;
using Empathica.TwitterChallenge.Web.Extensions;
using Empathica.TwitterChallenge.Web.ViewModels;
using Empathica.TwitterChallenge.Wires.Config;
using Spring.Social.OAuth1;
using Spring.Social.Twitter.Api;

namespace Empathica.TwitterChallenge.Web.Controllers
{
    public class TwitterController : BaseController
    {
        private IOAuth1ServiceProvider<ITwitter> _twitterProvider;

        public TwitterController()
        {
        }

        internal TwitterController(Container container, ISessionAdapter sessionAdapter)
            : base(container, sessionAdapter)
        {
        }

        private IOAuth1ServiceProvider<ITwitter> TwitterProvider
        {
            get
            {
                if (_twitterProvider == null)
                {
                    _twitterProvider = Container.TwitterServiceFactory();
                }
                return _twitterProvider;
            }
        }

        [GET("{id:int}")]
        public ActionResult View(int id)
        {
            TwitterStatus twitterStatus = Repository.GetStatusById(id);
            return View(twitterStatus);
        }

        [GET("")]
        public ActionResult Send()
        {
            return View(new StatusViewModel());
        }

        [GET("Twitter/SignIn")]
        public ActionResult SignIn()
        {
            //get absolute link to Callback action
            string url = Request.Url.GetLeftPart(UriPartial.Authority) + "/Twitter/Callback";
            OAuthToken requestToken = TwitterProvider
                .OAuthOperations
                .FetchRequestTokenAsync(url, null)
                .Result;

            Session.SetTwitterRequestToken(requestToken);

            //redirect to twitter service for auth
            return Redirect(
                TwitterProvider
                    .OAuthOperations
                    .BuildAuthenticateUrl(requestToken.Value, null));
        }

        [GET("Twitter/Callback")]
        public ActionResult Callback(string oauth_verifier)
        {
            OAuthToken requestToken = Session.GetTwitterRequestToken();
            var authorizedRequestToken = new AuthorizedRequestToken(requestToken, oauth_verifier);
            OAuthToken token =
                TwitterProvider
                    .OAuthOperations
                    .ExchangeForAccessTokenAsync(authorizedRequestToken, null)
                    .Result;

            Session.SetTwitterAccessToken(token);

            //post action
            string status = Session.GetTwitterNewStatus();
            if (!string.IsNullOrEmpty(status))
            {
                return SendUpdate(new StatusViewModel {Status = status}, token);
            }

            return Content("Yes");
        }

        //[ValidateAntiForgeryToken]
        [ValidateInput(false)]
        [POST("Twitter/UpdateStatus")]
        public ActionResult UpdateStatus(StatusViewModel model)
        {
            OAuthToken token = Session.GetTwitterAccessToken();
            if (token == null)
            {
                Session.SetTwitterStatus(model.Status);
                return RedirectToAction("SignIn");
            }
            return SendUpdate(model, token);
        }

        private ActionResult SendUpdate(StatusViewModel model, OAuthToken token)
        {
            if (!TryValidateModel(model))
            {
                return View("Send", model);
            }

            var twitterStatus = new TwitterStatus {Text = model.Status};
            //unit of work for db
            using (IRepository repository = Container.RepositoryFactory())
            {
                twitterStatus = repository.AddStatus(twitterStatus);
            }

            //twitter service operation
            ITwitter twitterClient = TwitterProvider.GetApi(token.Value, token.Secret);
            twitterClient.TimelineOperations.UpdateStatusAsync(twitterStatus.Text);

            return RedirectToAction("View", new {id = twitterStatus.Id});
        }
    }
}