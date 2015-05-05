-------------------------------------------------------
-- Title            : [dbo].[SearchView]
-- Description      : View for Searching the Communities and Contents
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-sagna
-- Date             : 29 Dec 2011
-- 
-------------------------------------------------------
CREATE VIEW [dbo].[SearchView]
AS
    SELECT    [dbo].[CommunitiesView].[CommunityID] AS [ID]
            , [dbo].[CommunitiesView].[CommunityName] AS [Name]
            , [dbo].[CommunitiesView].[Description]
            , [dbo].[CommunitiesView].[DistributedBy]
            , [dbo].[CommunitiesView].[CreatedByID] AS [ProducerId]
            , [dbo].[CommunitiesView].[ProducedBy] AS [Producer]
            , NULL AS [ParentID]
            , NULL AS [ParentName]
            , NULL AS [ParentType]
            , [dbo].[CommunitiesView].[AverageRating] AS [Rating]
            , [dbo].[CommunitiesView].[RatedPeople]
            , [dbo].[CommunitiesView].[Tags]
            , [dbo].[CommunitiesView].[ThumbnailID]
            , NULL AS [Filename]
            , NULL AS [ContentAzureID]
            , NULL AS [ContentUrl]
            , 0 AS [ContentType]
            , NULL AS [Citation]
            , [dbo].[CommunitiesView].[CategoryID] AS [Category]
            , 'Community' AS [Entity]
            , [dbo].[CommunitiesView].[AccessType]
            , [dbo].GetCommunityUsers([dbo].[CommunitiesView].[CommunityID]) AS [Users]
    FROM    [dbo].[CommunitiesView] 
    WHERE [dbo].[CommunitiesView].[CommunityTypeID] = 1
        UNION
    SELECT    [dbo].[ContentsView].[ContentID] AS [ID]
            , [dbo].[ContentsView].[Title] AS [Name]
            , [dbo].[ContentsView].[Description]
            , [dbo].[ContentsView].[DistributedBy]
            , [dbo].[ContentsView].[CreatedByID]  AS [ProducerId]
            , [dbo].[ContentsView].[ProducedBy] AS [Producer]
            , [dbo].[ContentsView].[CommunityID] AS [ParentID]
            , [dbo].[ContentsView].[CommunityName] AS [ParentName]
            , [dbo].[ContentsView].[CommunityTypeID] AS [ParentType]
            , [dbo].[ContentsView].[AverageRating] AS [Rating]
            , [dbo].[ContentsView].[RatedPeople]
            , [dbo].[ContentsView].[Tags]
            , [dbo].[ContentsView].[ThumbnailID]
            , [dbo].[ContentsView].[Filename]
            , [dbo].[ContentsView].[ContentAzureID]
            , [dbo].[ContentsView].[ContentUrl]
            , [dbo].[ContentsView].[TypeID] AS [ContentType]
            , [dbo].[ContentsView].[Citation]
            , [dbo].[ContentsView].[CategoryID] AS [Category]
            , 'Content' AS [Entity]
            , [dbo].[ContentsView].[AccessType]
            , [dbo].GetCommunityUsers([dbo].[ContentsView].[CommunityID]) AS [Users]
    FROM    [dbo].[ContentsView]