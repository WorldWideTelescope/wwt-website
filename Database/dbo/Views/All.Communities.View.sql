-------------------------------------------------------
-- 
-- Title            : [dbo].[AllCommunitiesView]
-- Description      : View for retrieving details about all the Communities, including the deleted ones.
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-sagna
-- Date             : 02 Jan 2012
-- 
-------------------------------------------------------
CREATE VIEW [dbo].[AllCommunitiesView]
AS
SELECT  [dbo].[Community].[CommunityID] As [CommunityID]
        , [dbo].[Community].[Name] As [CommunityName]
        , [dbo].[Community].[Description]
        , [dbo].[Category].[CategoryID] AS [CategoryID]
        , [dbo].[Category].[Name] AS [CategoryName]
        , [dbo].[Community].[DistributedBy]
        , [dbo].[Community].[ThumbnailID]
        , [dbo].[Community].[CreatedByID]
        , [dbo].[Community].[CommunityTypeID]
        , [dbo].[Community].[ViewCount]
        , [dbo].[User].[FirstName] + ' ' + [dbo].[User].[LastName] AS [ProducedBy]
        , [dbo].[Community].[CreatedDatetime]
        , [dbo].[Community].[ModifiedDatetime] AS [LastUpdatedDatetime]
        , [Parent].[CommunityID] AS [ParentID]
        , [Parent].[Name] AS [ParentName]
        , [dbo].GetCommunityAccessType([dbo].[Community].[CommunityID]) AS [AccessType]
        , [dbo].GetAverageCommunityRating([dbo].[Community].[CommunityID]) AS [AverageRating]
        , [dbo].GetCommunityRatedPeopleCount([dbo].[Community].[CommunityID]) AS [RatedPeople]
        , [dbo].GetCommunityTags([dbo].[Community].[CommunityID]) AS [Tags]
        , [dbo].[Community].[IsDeleted]
        , [dbo].[Community].[IsOffensive]
FROM    [dbo].[Community] 
        INNER JOIN [dbo].[Category] 
            ON [dbo].[Community].[CategoryID] = [dbo].[Category].[CategoryID]
        INNER JOIN [dbo].[User]
            ON [dbo].[User].[UserID] = [dbo].[Community].[CreatedByID]
        LEFT OUTER JOIN [dbo].[CommunityRelation]
            ON [dbo].[Community].[CommunityID] = [dbo].[CommunityRelation].[ChildCommunityID]
        LEFT OUTER JOIN [dbo].[Community] AS [Parent]
            ON [dbo].[CommunityRelation].[ParentCommunityID] = [Parent].[CommunityID]