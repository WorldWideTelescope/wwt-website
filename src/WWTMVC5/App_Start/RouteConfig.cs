using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WWTMVC5
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ignore wwtweb/ as this is handled by WwtWebHttpHandler
            routes.IgnoreRoute("{*wwtaspx}", new { wwtaspx = @"wwtweb/.*" });
            routes.IgnoreRoute("{*tour}", new { tour = @"GetTourFile2?\.aspx" });

            routes.MapRoute(
                "ExperienceIt",
                "experienceIt/experienceit.aspx",
                new { controller = "Default", action = "Index" }
            );

            routes.MapMvcAttributeRoutes();
        }
    }
}
