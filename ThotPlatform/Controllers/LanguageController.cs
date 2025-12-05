using System.Web;
using System.Web.Mvc;

namespace ThotPlatform.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language/Change
        public ActionResult Change(string lang)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                // Stocker la langue dans un cookie
                HttpCookie cookie = new HttpCookie("UserLanguage");
                cookie.Value = lang;
                cookie.Expires = System.DateTime.Now.AddYears(1);
                Response.Cookies.Add(cookie);
                
                // Stocker aussi dans la session
                Session["Language"] = lang;
            }
            
            // Rediriger vers la page precedente
            string returnUrl = Request.UrlReferrer?.ToString() ?? Url.Action("Index", "Home");
            return Redirect(returnUrl);
        }
    }
}

