using Azure.Identity;
using Microsoft.Practices.Unity;
using System;
using System.Configuration;
using System.Linq;
using WWT.Providers;
using WWTMVC5.Models;
using WWTMVC5.Repositories;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services;
using WWTMVC5.Services.Interfaces;
using WWTWebservices;
using WWTWebservices.Azure;

namespace WWTMVC5
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        private static readonly Lazy<IUnityContainer> _container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return _container.Value;
        }

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        private static void RegisterTypes(IUnityContainer container)
        {
            RegisterRequestProviders(container);
            RegisterPlateFileProvider(container);

            RegisterEarthOnlineEntities(container);
            RegisterRepositories(container);
            RegisterServices(container);

            container.RegisterType<IFileNameHasher, Net4x32BitFileNameHasher>(new ContainerControlledLifetimeManager());
        }

        private static void RegisterRequestProviders(IUnityContainer container)
        {
            var types = typeof(RequestProvider).Assembly.GetTypes()
                .Where(t => !t.IsAbstract && typeof(RequestProvider).IsAssignableFrom(t));

            foreach (var type in types)
            {
                container.RegisterType(type);
            }
        }

        private static void RegisterPlateFileProvider(IUnityContainer container)
        {
            if (ConfigReader<bool>.GetSetting("UseAzurePlateFiles"))
            {
                var options = new AzurePlateTilePyramidOptions
                {
                    StorageUri = ConfigurationManager.AppSettings["AzurePlateFileContainer"]
                };

                container.RegisterInstance<IPlateTilePyramid>(new AzurePlateTilePyramid(options, new DefaultAzureCredential()));
            }
            else
            {
                container.RegisterType<IPlateTilePyramid, ConfigurationManagerFilePlateTilePyramid>(new ContainerControlledLifetimeManager());
            }
        }

        private static void RegisterEarthOnlineEntities(IUnityContainer container)
        {
            var earthSettings = ConfigReader<string>.GetSetting("EarthOnlineEntities") ?? string.Empty;
            var manager = new PerRequestLifetimeManager();
            var constructor = new InjectionConstructor(earthSettings);

            container.RegisterType<EarthOnlineEntities>(manager, constructor);
        }

        /// <summary>
        /// Registers the required repositories which is required for all the controllers.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        private static void RegisterRepositories(IUnityContainer container)
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

        /// <summary>
        /// Registers the required services which is required for all the controllers.
        /// </summary>
        /// <param name="container">Instance of unity container</param>
        private static void RegisterServices(IUnityContainer container)
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
