using System.Globalization;
using System.Resources;
using System.Threading;
using System.Web;

namespace ThotPlatform.Utils
{
    public static class TranslationHelper
    {
        private static ResourceManager _resourceManager = new ResourceManager("ThotPlatform.Resources.Translations", typeof(TranslationHelper).Assembly);

        public static string T(string key)
        {
            var lang = GetCurrentLanguage();
            var culture = new CultureInfo(lang == "en" ? "en-US" : lang == "es" ? "es-ES" : "fr-FR");
            
            try
            {
                return _resourceManager.GetString(key, culture) ?? key;
            }
            catch
            {
                return key;
            }
        }

        public static string GetCurrentLanguage()
        {
            if (HttpContext.Current != null)
            {
                var session = HttpContext.Current.Session;
                var request = HttpContext.Current.Request;
                
                if (session?["Language"] != null)
                {
                    return session["Language"].ToString();
                }
                
                if (request.Cookies["UserLanguage"] != null)
                {
                    return request.Cookies["UserLanguage"].Value;
                }
            }
            
            return "fr";
        }

        public static void SetCurrentLanguage(string lang)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session["Language"] = lang;
                
                var cookie = new HttpCookie("UserLanguage");
                cookie.Value = lang;
                cookie.Expires = System.DateTime.Now.AddYears(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
    }
}

