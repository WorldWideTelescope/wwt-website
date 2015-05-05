-------------------------------------------------------
-- 
-- Title            : [dbo].[FeaturedCommunitiesView]
-- Description      : View for retrieving featured communities
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-vichan
-- Date             : 22 Dec 2011
-- 
-------------------------------------------------------
CREATE VIEW [dbo].[FeaturedCommunitiesView]
AS
SELECT  [dbo].[CommunitiesView].[CommunityID]
        , [CommunityName]
        , [Description]
        , [dbo].[CommunitiesView].[CategoryID]
        , [CategoryName]
        , [DistributedBy]
        , [ThumbnailID]
        , [CreatedByID]
        , [CommunityTypeID]
        , [ViewCount]
        , [ProducedBy]
        , [CreatedDatetime]
        , [LastUpdatedDatetime]
        , [ParentID]
        , [ParentName]
        , [AccessType]
        , [AverageRating]
        , [RatedPeople]
        , [Tags]
        , ISNULL([dbo].[FeaturedCommunities].[CategoryID], 0) [FeaturedCategoryID]
        , [SortOrder]
FROM    [dbo].[CommunitiesView]
			INNER JOIN [dbo].[FeaturedCommunities]
				ON [dbo].[FeaturedCommunities].[CommunityID] = [dbo].[CommunitiesView].[CommunityID]