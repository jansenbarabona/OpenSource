using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VehicleRegistration.Tools
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        public UserEntityEnum[] AllowedUserEntity { get; set; }
        public UserRoleEnum[] AllowedUserRole { get; set; }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {

            if (httpContext.Session["VRCurrentUser"] != null && AllowedUserEntity != null && AllowedUserEntity.Contains((UserEntityEnum)CurrentUser.Details.UserEntityID))
                return true;
            else if (httpContext.Session["VRCurrentUser"] != null && AllowedUserRole != null && AllowedUserRole.Contains((UserRoleEnum)CurrentUser.Details.UserRoleID))
                return true;
            else
                return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {

            filterContext.Result = new RedirectToRouteResult(new
                RouteValueDictionary(new { controller = "Home", Action = "Dashboard" }));


        }
    }
}