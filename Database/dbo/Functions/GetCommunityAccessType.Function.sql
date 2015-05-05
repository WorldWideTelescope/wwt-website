-------------------------------------------------------
-- 
-- Title            : [dbo].[GetCommunityAccessType]
-- Description      : This function returns the access type of the community or folder
-- Input            : ID of the Community or folder
-- Output           : Access Type of the Community or folder
-- Author           : v-sagna
-- Date             : 21 Sep 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].[GetCommunityAccessType]
(
    @CommunityID BIGINT
)
RETURNS VARCHAR(250)
AS
BEGIN
    DECLARE @AccessType VARCHAR(250)
    DECLARE @AccessTypeID INT
    DECLARE @ParentCommunityID BIGINT
    DECLARE @IsCommunity INT

    SELECT 
        @ParentCommunityID = dbo.CommunityRelation.ParentCommunityID,
        @AccessType = dbo.AccessType.Name,
        @IsCommunity = dbo.Community.CommunityTypeID
    FROM Community
        LEFT OUTER JOIN dbo.CommunityRelation
            ON dbo.CommunityRelation.ChildCommunityID = dbo.Community.CommunityID
        INNER JOIN dbo.AccessType
            ON dbo.AccessType.AccessTypeID = dbo.Community.AccessTypeID
        WHERE dbo.Community.CommunityID=@CommunityID

    -- If current entity itself is community or Private (even if it is folder), return its AccessType.
    -- If current entity is folder and access type is public, check all the way to its parent community and
    -- their access types. If any of the folder is private before reaching the parent community, return Private.
    -- If able to reach the Parent community, return its access type.
    IF (@AccessType = 'Public' AND @IsCommunity = 2 AND @ParentCommunityID IS NOT NULL)
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
                WHERE ParentCommunityID = @ParentCommunityID
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
    
    RETURN @AccessType
END