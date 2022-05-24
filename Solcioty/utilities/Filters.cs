using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Models;
using Newtonsoft.Json;
using Services;

namespace web
{
    public class AuthorizeUser : AuthorizeAttribute
    {

        public AuthorizeUser()
        {
            View = "error";
            Master = String.Empty;
        }

        public String View { get; set; }
        public String Master { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            CheckIfUserIsAuthenticated(filterContext);
        }

        private void CheckIfUserIsAuthenticated(AuthorizationContext filterContext)
        {
            // If Result is null, we're OK: the user is authenticated and authorized. 
            if (filterContext.Result == null)
                return;

            // If here, you're getting an HTTP 401 status code. In particular,
            // filterContext.Result is of HttpUnauthorizedResult type. Check Ajax here. 
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (String.IsNullOrEmpty(View))
                    return;
                var result = new ViewResult { ViewName = View, MasterName = Master };
                filterContext.Result = result;
            }
        }
        //#pending
    }

    [AttributeUsage(AttributeTargets.Class |
    AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionTimeoutFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            // If the browser session or authentication session has expired...
            if (session.IsNewSession || session["UserData"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, return result as a simple string, 
                    // and inform calling JavaScript code that a user should be redirected.
                    //JsonResult result = Json("SessionTimeout", "text/html");
                    //filterContext.Result = result;
                }
                else
                {

                    // For round-trip requests,
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "Controller", "Account" },
                { "Action", "Login" }
                    });
                }
            }
            base.OnActionExecuting(filterContext);
        }
        //other methods...
    }


    [AttributeUsage(AttributeTargets.Class |
   AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class HasPermissionFilter : ActionFilterAttribute
    {
        public string PermissionCode { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            var userData = session["UserData"] as UserData;
            var permissionType = AccessbilityService.GetPermission(userData.RoleCode, PermissionCode);
            // If the browser session or authentication session has expired...
            if (permissionType == Models.Enums.PermissionType.None)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, return result as a simple string, 
                    // and inform calling JavaScript code that a user should be redirected.
                    //JsonResult result = Json("SessionTimeout", "text/html");
                    //filterContext.Result = result;
                }
                else
                {
                    //session.Clear();
                    // For round-trip requests,
                    filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                { "Controller", "Account" },
                { "Action", "Unauthorized" }
                    });
                }
            }
            base.OnActionExecuting(filterContext);
        }
        //other methods...
    }
}