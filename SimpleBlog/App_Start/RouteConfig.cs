using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SimpleBlog {
    public class RouteConfig {
        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Category",
                url: "category/{category}",
                defaults: new { controller = "Blog", action = "Category" }
            );

            routes.MapRoute(
                name: "Tag",
                url: "tag/{tag}",
                defaults: new { controller = "Blog", action = "Tag" }
            );

            routes.MapRoute(
                name: "Post",
                url: "archive/{year}/{month}/{title}",
                defaults: new { controller = "Blog", action = "Post" }
            );

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Admin", action = "Login" }
            );

            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Admin", action = "Logout" }
            );

            routes.MapRoute(
                name: "Manage",
                url: "manage",
                defaults: new { controller = "Admin", action = "Manage" }
            );

            routes.MapRoute(
                name: "AdminAction",
                url: "Admin/{action}",
                defaults: new { controller = "Admin", action = "Login" }
            );

            routes.MapRoute(
                name: "Action",
                url: "{action}",
                defaults: new { controller = "Blog", action = "Posts" }
            );
        }
    }
}
