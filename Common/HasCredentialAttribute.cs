using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Common
{
    public class HasCredentialAttribute : AuthorizeAttribute
    {

        public string ControllerName { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            UserLogin sesion = (UserLogin)HttpContext.Current.Session["USER_SESSION"];
            if (sesion == null)
                return false;
            List<string> privilegeLevels = GetCredentialByLoggeInUser(sesion.UserName);
            if (privilegeLevels.Contains(ControllerName))
                return true;
            else
                return false;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            UserLogin sesion = (UserLogin)HttpContext.Current.Session["USER_SESSION"];
            if (sesion != null)
            {
                filterContext.Result = new ViewResult
                {
                    ViewName = "~/Views/Shared/401.cshtml"
                };
            }
            else
            {
                UrlHelper urlHelper = new UrlHelper(filterContext.RequestContext);
                filterContext.Result = new RedirectResult(urlHelper.Action("Index", "Login"));
            }
        }
        private List<string> GetCredentialByLoggeInUser(string userName)
        {
            var credentials = (List<string>)HttpContext.Current.Session["CEDENTIALS_SESSION"];
            return credentials;
        }
    }
}
