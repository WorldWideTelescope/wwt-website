-------------------------------------------------------
-- Title            : [dbo].[GetCommunityUsers]
-- Description      : This function retrieves the users who are having access to the Community
-- Input            : ID of the Community.
-- Output           : Users of the community.
-- Author           : v-sagna
-- Date             : 29 Dec 2011
-------------------------------------------------------
CREATE FUNCTION [dbo].[GetCommunityUsers]
(
    @CommunityID BIGINT
)
RETURNS VARCHAR(1024)
AS
BEGIN
    DECLARE @UserIds VARCHAR(1024)

    SELECT @UserIds = COALESCE(@UserIds + '~', '~') + CAST(UserID AS VARCHAR(1024))
    FROM [dbo].[UserCommunities]
    WHERE [dbo].[UserCommunities].[CommunityId] = @CommunityID

    SET @UserIds = COALESCE(@UserIds + '~', '~')

    RETURN @UserIds
END