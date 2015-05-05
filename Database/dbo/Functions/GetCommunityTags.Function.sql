-------------------------------------------------------
-- 
-- Title            : [dbo].[GetCommunityTags]
-- Description		: This function can be used to retrieve tags of a Community
-- Input            : ID of the Community.
-- Output			: All tags of the community separated by comma.
-- Author			: v-vichan
-- Date             : 09 Aug 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].[GetCommunityTags]
(
	@CommunityID BIGINT
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @Tags NVARCHAR(4000)

	SELECT	@Tags = coalesce(@Tags + ',' , '') + dbo.[Tag].[Name] 
	FROM	dbo.[Community] 
			LEFT OUTER JOIN dbo.[CommunityTags]
				ON dbo.[CommunityTags].[CommunityID] = dbo.[Community].[CommunityID]
			LEFT OUTER JOIN dbo.[Tag]
				ON dbo.[Tag].[TagID] = dbo.[CommunityTags].[TagID]
	WHERE dbo.[Community].[CommunityID] = @CommunityID

	RETURN @Tags
END
GO
