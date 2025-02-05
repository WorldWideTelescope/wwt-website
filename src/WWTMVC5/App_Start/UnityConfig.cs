using Microsoft.Extensions.Logging;
using System;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using Unity.Lifetime;
using WWTMVC5.Models;
using WWTMVC5.Repositories;
using WWTMVC5.Repositories.Interfaces;
using WWTMVC5.Services;
using WWTMVC5.Services.Interfaces;

namespace WWTMVC5
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        public static IUnityContainer Container { get; private set; }

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer ConfigureContainer(IServiceProvider provider)
        {
            var container = new UnityContainer();
            RegisterTypes(container, provider);
            Container = container;

            return Container;
        }

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        private static void RegisterTypes(IUnityContainer container, IServiceProvider provider)
        {
            RegisterEarthOnlineEntities(container);
            RegisterRepositories(container);
            RegisterServices(container);
            MapServicesFromProvider(container, provider);
        }

        private static void MapServicesFromProvider(IUnityContainer container, IServiceProvider provider)
        {
            container.RegisterFactory(typeof(ILogger<>), null, (_, type, __) => provider.GetService(type));
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
