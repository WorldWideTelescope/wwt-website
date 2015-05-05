-------------------------------------------------------
-- 
-- Title            : [dbo].[GetContentAccessType]
-- Description      : This function returns the access type of the content
-- Input            : ID of the Content.
-- Output           : Access Type of the Content
-- Author           : v-sagna
-- Date             : 21 Sep 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].[GetContentAccessType]
(
    @ContentID BIGINT
)
RETURNS VARCHAR(250)
AS
BEGIN
    DECLARE @AccessType VARCHAR(250)
    DECLARE @AccessTypeID INT
    DECLARE @ParentCommunityID BIGINT
    DECLARE @IsCommunity INT

    SELECT 
        @ParentCommunityID = CommunityID,
        @AccessType = dbo.AccessType.Name
    FROM Content
        INNER JOIN dbo.CommunityContents
            ON dbo.CommunityContents.ContentID = dbo.Content.ContentID
        INNER JOIN dbo.AccessType
            ON dbo.AccessType.AccessTypeID = dbo.Content.AccessTypeID
        WHERE dbo.Content.ContentID=@ContentID

    -- If Content itself is Private, return the access type as "Private".
    IF (@AccessType = 'Public')
    BEGIN
        -- If Content in public, then check the immediate parent (folder or community).
        SELECT @IsCommunity=CommunityTypeID, @AccessType=AccessType FROM CommunitiesView WHERE CommunityID = @ParentCommunityID AND CommunityTypeID != 3

        -- If immediate parent (folder or community) is Private, return the access type as "Private". Or if the
        -- immediate parent is a community, then return its AccessType even if it is private or public.
        IF (@AccessType = 'Public' AND @IsCommunity = 2)
        BEGIN
            WITH CommonTableExpression AS 
            (
                SELECT 
                    dbo.CommunityRelation.ParentCommunityID, 
                    dbo.CommunityRelation.ChildCommunityID,
                    dbo.Community.AccessTypeID,
                    dbo.Community.CommunityTypeID AS CommunityTypeID
                FROM dbo.CommunityRelation
                    INNER JOIN dbo.Community
                        ON dbo.CommunityRelation.ParentCommunityID = dbo.Community.CommunityID
                    WHERE ChildCommunityID = @ParentCommunityID
                            UNION ALL 
                SELECT 
                    dbo.CommunityRelation.ParentCommunityID,
                    dbo.CommunityRelation.ChildCommunityID ,
                    dbo.Community.AccessTypeID,
                    dbo.Community.CommunityTypeID AS CommunityTypeID
                FROM dbo.CommunityRelation 
                    INNER JOIN dbo.Community
                        ON dbo.CommunityRelation.ParentCommunityID = dbo.Community.CommunityID
                    INNER JOIN CommonTableExpression c ON c.ParentCommunityID = dbo.CommunityRelation.ChildCommunityID
                WHERE c.AccessTypeID != 1 AND c.CommunityTypeID != 1
            )
            SELECT @AccessTypeID = AccessTypeID FROM CommonTableExpression OPTION (MAXRECURSION 100)
            IF (@AccessTypeID = 1)
                SET @AccessType = 'Private'
            ELSE IF (@AccessTypeID = 2)
                SET @AccessType = 'Public'
        END
    END
    
    RETURN @AccessType
END