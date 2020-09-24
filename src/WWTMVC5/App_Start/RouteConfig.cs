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
			//routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
			//routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
			//routes.IgnoreRoute("{*allxml}", new { allxml = @".*\.xml(/.*)?" });
			//routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
			//routes.IgnoreRoute("{*models}", new { models = @"(.*/)?models(/.*)?" });
			//routes.IgnoreRoute("{*content}", new { models = @"(.*/)?content(/.*)?" });

            routes.MapRoute(
                "ExperienceIt",
                "experienceIt/experienceit.aspx",
                new { controller = "Default", action = "Index" }
            );

            
			routes.MapMvcAttributeRoutes();
			//routes.MapRoute(
			//	"About",
			//	"About/{page}",
			//	new { controller = "Default", action = "AboutResult" }
			//);
			//routes.MapRoute(
			//	"Contact",
			//	"Contact/{page}",
			//	new { controller = "Default", action = "ContactResult" }
			//);
			//routes.MapRoute(
			//	"Developers",
			//	"Developers/{page}",
			//	new { controller = "Default", action = "DevelopersResult" }
			//);
			//routes.MapRoute(
			//	"Download",
			//	"Download/{page}",
			//	new { controller = "Default", action = "DownloadResult" }
			//);
			//routes.MapRoute(
			//	"Educators",
			//	"Educators/{page}",
			//	new { controller = "Default", action = "EducatorsResult" }
			//);
			//routes.MapRoute(
			//	"Explorers",
			//	"Explorers/{page}",
			//	new { controller = "Default", action = "ExplorersResult" }
			//);
			//routes.MapRoute(
			//	"Eyewire",
			//	"Eyewire/{page}",
			//	new { controller = "Default", action = "EyewireResult" }
			//);
			//routes.MapRoute(
			//	"Interact",
			//	"Interact/{page}",
			//	new { controller = "Default", action = "InteractResult" }
			//);
			//routes.MapRoute(
			//	"Layerscape",
			//	"Layerscape/{page}",
			//	new { controller = "Default", action = "LayerscapeResult" }
			//);
			//routes.MapRoute(
			//	"Learn",
			//	"Learn/{page}",
			//	new { controller = "Default", action = "LearnResult" }
			//);
			//routes.MapRoute(
			//	"Museums",
			//	"Museums/{page}",
			//	new { controller = "Default", action = "MuseumsResult" }
			//);
			//routes.MapRoute(
			//	"Planetariums",
			//	"Planetariums/{page}",
			//	new { controller = "Default", action = "PlanetariumsResult" }
			//);
			//routes.MapRoute(
			//	"Researchers",
			//	"Researchers/{page}",
			//	new { controller = "Default", action = "ResearchersResult" }
			//);
			//routes.MapRoute(
			//	"Support",
			//	"Support/{page}",
			//	new { controller = "Default", action = "SupportResult" }
			//);
			//routes.MapRoute(
			//	"Terms",
			//	"Terms/{page}",
			//	new { controller = "Default", action = "TermsResult" }
			//);
		}
	}
}
