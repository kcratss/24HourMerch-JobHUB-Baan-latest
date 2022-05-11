using KEN.AppCode;
using KEN_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KEN.Filters
{
    public class UserAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedroles;


        public UserAuthorizeAttribute(params string[] roles)
        {
            this.allowedroles = roles;

        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            var userEmail = DataBaseCon.ActiveUser();

            if (userEmail != null)
            {
                using (var context = new KENNEWEntities())
                {
                    var userRole = context.tblusers.Where(x => x.email == userEmail && x.UserRole == "Online").FirstOrDefault();
                    if (userRole != null)
                    {
                        var role = userRole.UserRole;
                        return true;
                    }

                }

            }


            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "User" },
                    { "action", "Login" }
               });
        }


    }
}