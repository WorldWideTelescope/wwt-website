using System;
using System.CodeDom;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using WWTMVC5.Models;
using WWTMVC5.Repositories;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services;
using WWTMVC5.Services.Interfaces;
using System.IdentityModel.Services;

namespace WWTMVC5
{
	public class MvcApplication : HttpApplication
	{
        private static UnityContainer _container;

        /// <summary>
        /// Gets the parent unity container.
        /// </summary>
        public static UnityContainer ParentUnityContainer
        {
            get
            {
                return _container;
            }
        }

		public const String WurflDataFilePath = "./App_Data/wurfl-latest.zip";
		
        protected void Application_Start()
		{
           
            RegisterUnityContainer(); 

            AutoMapperSettings.RegisterControllerAutoMappers();
            AutoMapperSettings.RegisterServiceAutoMappers();
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			//DateTime time = DateTime.Now;
			var wurflDataFile = HttpContext.Current.Server.MapPath(WurflDataFilePath);

			//DateTime time2 = DateTime.Now;
			//var wurfltime = time.Millisecond - time2.Millisecond;
			//var breakhere = true;

		}

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            var cryptoEx = error as CryptographicException;
            if (cryptoEx != null)
            {
                FederatedAuthentication.SessionAuthenticationModule.SignOut();
                SessionWrapper.Clear();
                Server.ClearError();
            }
        }

        /// <summary>
        /// Creates an instance of UnityContainer and registers the instances which needs to be injected
        /// to Controllers/Views/Services, etc.
        /// </summary>
        private static void RegisterUnityContainer()
        {
            _container = new UnityContainer();
            var earthSettings = ConfigReader<string>.GetSetting("EarthOnlineEntities");
            var manager = new PerRequestLifetimeManager();
            var constructor = new InjectionConstructor(earthSettings);
            _container.RegisterType<EarthOnlineEntities>(manager,constructor);

            RegisterRepositories(_container);
            RegisterServices(_container);

            DependencyResolver.SetResolver(new UnityDependencyResolver(_container));
        }

        /// <summary>
        /// Registers the required repositories which is required for all the controllers.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "TODO: Need to refractor.")]
        private static void RegisterRepositories(UnityContainer container)
        {
            if (container != null)
            {
                container.RegisterType<IBlobDataRepository, BlobDataRepository>(new ContainerControlledLifetimeManager());
                container.RegisterType<ICommunitiesViewRepository, CommunitiesViewRepository>(new PerRequestLifetimeManager());
                container.RegisterType<ICommunityCommentRepository, CommunityCommentRepository>(new PerRequestLifetimeManager());
                container.RegisterType<ICommunityRepository, CommunityRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IContentCommentsRepository, ContentCommentsRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IContentsViewRepository, ContentsViewRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<Tag>, RepositoryBase<Tag>>(new PerRequestLifetimeManager());
                container.RegisterType<IContentRepository, ContentRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<CommunityRelation>, RepositoryBase<CommunityRelation>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<CommunityContents>, RepositoryBase<CommunityContents>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<TopCategoryEntities>, RepositoryBase<TopCategoryEntities>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<UserCommunities>, RepositoryBase<UserCommunities>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<ContentRatings>, RepositoryBase<ContentRatings>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<CommunityRatings>, RepositoryBase<CommunityRatings>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<OffensiveContent>, RepositoryBase<OffensiveContent>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<OffensiveCommunities>, RepositoryBase<OffensiveCommunities>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<StaticContent>, RepositoryBase<StaticContent>>(new PerRequestLifetimeManager());
                container.RegisterType<IUserRepository, UserRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IUserCommunitiesRepository, UserCommunitiesRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<PermissionRequest>, RepositoryBase<PermissionRequest>>(new PerRequestLifetimeManager());
                container.RegisterType<IQueueRepository, QueueRepository>(new ContainerControlledLifetimeManager());
                container.RegisterType<IRepositoryBase<PermissionRequest>, RepositoryBase<PermissionRequest>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<InviteRequestsView>, RepositoryBase<InviteRequestsView>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<InviteRequest>, RepositoryBase<InviteRequest>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<FeaturedCommunitiesView>, RepositoryBase<FeaturedCommunitiesView>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<FeaturedContentsView>, RepositoryBase<FeaturedContentsView>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<FeaturedCommunities>, RepositoryBase<FeaturedCommunities>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<FeaturedContents>, RepositoryBase<FeaturedContents>>(new PerRequestLifetimeManager());
                container.RegisterType<ISearchViewRepository, SearchViewRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<UserType>, RepositoryBase<UserType>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<AllCommunitiesView>, RepositoryBase<AllCommunitiesView>>(new PerRequestLifetimeManager());
                container.RegisterType<IRepositoryBase<AllContentsView>, RepositoryBase<AllContentsView>>(new PerRequestLifetimeManager());
                container.RegisterType<ICommunityTagsRepository, CommunityTagsRepository>(new PerRequestLifetimeManager());
                container.RegisterType<IContentTagsRepository, ContentTagsRepository>(new PerRequestLifetimeManager());
            }
        }
         
        /// <summary>
        /// Registers the required services which is required for all the controllers.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        private static void RegisterServices(UnityContainer container)
        {
            if (container != null)
            {
                container.RegisterType<IBlobService, BlobService>(new ContainerControlledLifetimeManager());
                container.RegisterType<ICommunityService, CommunityService>(new PerRequestLifetimeManager());
                container.RegisterType<IContentService, ContentService>(new PerRequestLifetimeManager());
                container.RegisterType<IEntityService, EntityService>(new PerRequestLifetimeManager());
                container.RegisterType<ICommentService, CommentService>(new PerRequestLifetimeManager());
                container.RegisterType<ISearchService, SearchService>(new PerRequestLifetimeManager());
                container.RegisterType<IProfileService, ProfileService>(new PerRequestLifetimeManager());
                container.RegisterType<IRatingService, RatingService>(new PerRequestLifetimeManager());
                container.RegisterType<IStaticContentService, StaticContentService>(new PerRequestLifetimeManager());
                container.RegisterType<IReportEntityService, ReportEntityService>(new PerRequestLifetimeManager());
                container.RegisterType<INotificationService, NotificationService>(new PerRequestLifetimeManager());
                container.RegisterType<IFeaturedEntityService, FeaturedEntityService>(new PerRequestLifetimeManager());
            }
        }
	}
}
