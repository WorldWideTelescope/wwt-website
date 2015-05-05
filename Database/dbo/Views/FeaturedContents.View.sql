-------------------------------------------------------
-- 
-- Title            : [dbo].[FeaturedContentsView]
-- Description      : View for retrieving featured contents
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-vichan
-- Date             : 22 Dec 2011
-- 
-------------------------------------------------------
CREATE VIEW [dbo].[FeaturedContentsView]
AS
SELECT  [dbo].[ContentsView].[ContentID]
		, [CreatedByID]
		, [TypeID]
		, [Title]
		, [Filename]
		, [Description]
		, [ContentAzureID]
		, [ContentAzureURL]
		, [DownloadCount]
		, [ThumbnailID]
		, [CreatedDatetime]
		, [ContentUrl]
		, [DistributedBy]
		, [Citation]
		, [TourRunLength]
		, [ProducedBy]
		, [LastUpdatedDatetime]
		, [CommunityID]
		, [CommunityName] 
		, [CommunityTypeID] 
		, [dbo].[ContentsView].[CategoryID]
		, [AccessType]        
		, [AverageRating]
		, [RatedPeople]
		, [Tags]
		, ISNULL([dbo].[FeaturedContents].[CategoryID], 0) [FeaturedCategoryID]
        , [SortOrder]
FROM    [dbo].[ContentsView]
			INNER JOIN [dbo].[FeaturedContents]
				ON [dbo].[FeaturedContents].[ContentID] = [dbo].[ContentsView].[ContentID]