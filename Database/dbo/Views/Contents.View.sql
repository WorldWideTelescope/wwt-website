-------------------------------------------------------
-- 
-- Title            : [dbo].[ContentsView]
-- Description      : View for retrieving featured contents
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-vichan
-- Date             : 09 Aug 2011
-- 
-------------------------------------------------------
CREATE VIEW [dbo].[ContentsView]
AS
SELECT  [dbo].[Content].[ContentID]
        , [dbo].[Content].[CreatedByID]
        , [dbo].[Content].[TypeID]
        , [dbo].[Content].[Title]
        , [dbo].[Content].[Filename]
        , [dbo].[Content].[Description]
        , [dbo].[Content].[ContentAzureID]
        , [dbo].[Content].[ContentAzureURL]
        , [dbo].[Content].[DownloadCount]
        , [dbo].[Content].[ThumbnailID]
        , [dbo].[Content].[CreatedDatetime]
        , [dbo].[Content].[ContentUrl]
        , [dbo].[Content].[DistributedBy]
        , [dbo].[Content].[Citation]
        , [dbo].[Content].[TourRunLength]
        , [dbo].[User].[FirstName] + ' ' + [dbo].[User].[LastName] AS [ProducedBy]
        , [dbo].[Content].[ModifiedDatetime] AS [LastUpdatedDatetime]
        , [dbo].[Community].[CommunityID] AS [CommunityID]
        , [dbo].[Community].[Name] AS [CommunityName] 
        , [dbo].[Community].[CommunityTypeID] AS [CommunityTypeID] 
        , [dbo].[Category].[CategoryID] AS [CategoryID]
        , [dbo].GetContentAccessType([dbo].[Content].[ContentID]) AS [AccessType]
        , [dbo].GetAverageContentRating([dbo].[Content].[ContentID]) AS [AverageRating]
        , [dbo].GetContentRatedPeopleCount([dbo].[Content].[ContentID]) AS [RatedPeople]
        , [dbo].GetContentTags([dbo].[Content].[ContentID]) AS [Tags]
FROM    [dbo].[Content] 
        INNER JOIN [dbo].[CommunityContents]
            ON [dbo].[CommunityContents].[ContentID] = [dbo].[Content].[ContentID]
        INNER JOIN [dbo].[Community]
            ON [dbo].[CommunityContents].[CommunityID] = [dbo].[Community].[CommunityID]
        INNER JOIN [dbo].[Category] 
            ON [dbo].[Content].[CategoryID] = [dbo].[Category].[CategoryID]
        INNER JOIN [dbo].[User]
            ON [dbo].[User].[UserID] = [dbo].[Content].[CreatedByID]
WHERE [dbo].[Content].[IsDeleted] = 'false' 
            AND [dbo].[Community].[IsDeleted] = 'false'