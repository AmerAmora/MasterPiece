using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MasterPiece
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute()
        {
            // Set the default error redirection URL
            ErrorUrl = "~/Views/Shared/NoAccess.cshtml";
        }

        public string ErrorUrl { get; set; }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // If the user is authenticated, but not authorized, redirect to the error page
                filterContext.Result = new ViewResult { ViewName = ErrorUrl };
            }
            else
            {
                // If the user is not authenticated, redirect to the login page
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }


}