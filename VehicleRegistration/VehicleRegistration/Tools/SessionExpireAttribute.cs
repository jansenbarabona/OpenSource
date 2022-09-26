using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VehicleRegistration.Tools
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //HttpContext ctx = HttpContext.Current;

            // check  sessions here
            if (HttpContext.Current.Session["VRCurrentUser"] == null)
            {
                string actionName = filterContext.ActionDescriptor.ActionName;
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

                HttpContext.Current.Session["RedirectFromLogin"] = "~/" + controllerName + "/" + actionName;

                filterContext.Result = new RedirectResult("~/User/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}