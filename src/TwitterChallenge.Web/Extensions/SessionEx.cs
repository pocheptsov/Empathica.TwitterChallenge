using Spring.Social.OAuth1;

namespace Empathica.TwitterChallenge.Web.Extensions
{
    public static class SessionEx
    {
        const string RequestTokenCode = "TwitterRequestToken";
        const string AccessTokenCode = "TwitterAccessToken";
        const string StatusCode = "TwitterStatus";

        public static OAuthToken GetTwitterRequestToken(this ISessionAdapter obj)
        {
            return obj.GetValueOrEmpty<OAuthToken>(RequestTokenCode);
        }
        
        public static void SetTwitterRequestToken(this ISessionAdapter obj, OAuthToken value)
        {
            obj.SetValue(RequestTokenCode, value);
        }

        public static OAuthToken GetTwitterAccessToken(this ISessionAdapter obj)
        {
            return obj.GetValueOrEmpty<OAuthToken>(AccessTokenCode);
        }

        public static void SetTwitterAccessToken(this ISessionAdapter obj, OAuthToken value)
        {
            obj.SetValue(AccessTokenCode, value);
        }

        public static string GetTwitterNewStatus(this ISessionAdapter obj)
        {
            return obj.GetValueOrEmpty<string>(StatusCode);
        }

        public static void SetTwitterStatus(this ISessionAdapter obj, string value)
        {
            obj.SetValue(StatusCode, value);
        }
    }
}
