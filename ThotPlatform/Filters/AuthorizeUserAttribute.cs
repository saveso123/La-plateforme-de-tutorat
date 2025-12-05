using System;
using System.Web;
using System.Web.Mvc;

namespace ThotPlatform.Filters
{
    /// <summary>
    /// Attribut d'autorisation personnalise pour verifier le type d'utilisateur
    /// </summary>
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public string UserType { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            // Verifier si l'utilisateur est authentifie
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            // Si aucun type specifie, autoriser tout utilisateur authentifie
            if (string.IsNullOrEmpty(UserType))
                return true;

            // Verifier le type d'utilisateur dans la session
            var sessionUserType = httpContext.Session["UserType"]?.ToString();
            
            return !string.IsNullOrEmpty(sessionUserType) && 
                   sessionUserType.Equals(UserType, StringComparison.OrdinalIgnoreCase);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Utilisateur connecte mais pas le bon type
                filterContext.Result = new RedirectResult("~/");
            }
            else
            {
                // Utilisateur non connecte
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Account", action = "Login", returnUrl = filterContext.HttpContext.Request.RawUrl }
                    )
                );
            }
        }
    }
}

