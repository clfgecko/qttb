using Model.ModelExtend;
using Simple.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = (UserLogin)Session["USER_SESSION"];
            if (session == null)
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }
            base.OnActionExecuting(filterContext);
        }


        public List<string> GetBottomRoleByController(string controllerName, List<MenuModel> menu)
        {
            var result = new List<string>();
            try
            {
                var cs = menu.FirstOrDefault(x => !string.IsNullOrEmpty(x.CONTROLLER_NAME) && x.CONTROLLER_NAME.ToUpper() == controllerName.ToUpper());
                if (cs != null && !string.IsNullOrEmpty(cs.Actions))
                    result = cs.Actions.Split('|').ToList();
            }
            catch (Exception ex)
            {
                result = new List<string>();
            }
            return result;
        }
    }
}