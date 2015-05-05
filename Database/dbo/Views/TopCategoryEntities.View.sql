-------------------------------------------------------
-- 
-- Title            : [dbo].[TopCategoryEntities]
-- Description      : View for retrieving top 2 communities and top 1 content from the all categories
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-sagna
-- Date             : 31 Jan 2012
-- 
-------------------------------------------------------
CREATE VIEW [dbo].[TopCategoryEntities]
AS
    SELECT ContentID AS ID, Title, Filename, TypeID, CategoryID, ThumbnailID, CommunityID, CommunityName, Tags, AverageRating, RatedPeople, CommunityTypeID, 'Content' AS EntityType FROM
        (SELECT ContentID, Title, Filename, TypeID, CategoryID, ThumbnailID, CommunityID, CommunityName, Tags, AverageRating, RatedPeople, CommunityTypeID
                ,ROW_NUMBER() OVER 
                    (PARTITION BY [dbo].[ContentsView].[CategoryID] ORDER BY [dbo].GetAverageContentRating([dbo].[ContentsView].[ContentID]) DESC)
                        AS RowNumber
            FROM [dbo].[ContentsView]
            WHERE [dbo].[ContentsView].[CategoryID] IN 
                    (   SELECT [dbo].[Category].[CategoryID] 
                        FROM [dbo].[Category])
                AND [dbo].[ContentsView].[AccessType] = 'Public'
        ) TopCommunities WHERE RowNumber = 1
    UNION
    SELECT CommunityID AS ID, Name AS Title, NULL AS FileName, NULL AS TypeID, CategoryID, ThumbnailID, NULL AS CommunityID, NULL AS CommunityName, NULL AS Tags, NULL AS AverageRating, NULL AS RatedPeople, NULL AS CommunityTypeID, 'Community' AS EntityType FROM 
        (SELECT [dbo].[Community].[Name]
                ,[dbo].[Community].[CommunityID]
                ,[dbo].[Category].[CategoryID] AS CategoryID
                ,[dbo].[Community].[ThumbnailID]
                ,[dbo].GetAverageCommunityRating([dbo].[Community].[CommunityID]) AS [AverageRating]
                ,[dbo].GetCommunityRatedPeopleCount([dbo].[Community].[CommunityID]) AS [RatedPeople]
                ,ROW_NUMBER() OVER 
                    (PARTITION BY [dbo].[Community].[CategoryID] ORDER BY [dbo].GetAverageCommunityRating([dbo].[Community].[CommunityID]) DESC)
                        AS RowNumber
            FROM [dbo].[Community]
                INNER JOIN [dbo].[Category]
                    ON [dbo].[Community].[CategoryID] = [dbo].[Category].[CategoryID]
            WHERE   [dbo].[Category].[IsDeleted] = 0
                AND [dbo].[Community].[IsDeleted] = 0
                AND [dbo].[Community].[CommunityTypeID] = 1 -- ID 1 Identifies community in master table
                AND [dbo].[Community].[AccessTypeID] = 2
            GROUP BY [dbo].[Category].[CategoryID]
                ,[dbo].[Community].[CategoryID]
                ,[dbo].[Community].[CommunityID]
                ,[dbo].[Community].[Name]
                ,[dbo].[Community].[ThumbnailID]
        ) TopCommunities 
        WHERE RowNumber < 3
