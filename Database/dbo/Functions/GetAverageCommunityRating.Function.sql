-------------------------------------------------------
-- 
-- Title            : [dbo].[GetAverageCommunityRating]
-- Description		: This function can be used to retrieve average rating of a Community
-- Input            : ID of the Community.
-- Output			: Average Rating of the community.
-- Author			: v-vichan
-- Date             : 09 Aug 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].[GetAverageCommunityRating]
(
	@CommunityID BIGINT
)
RETURNS DECIMAL(3,2)
AS
BEGIN
	DECLARE @AverageRating DECIMAL(3,2)

	SELECT @AverageRating = AVG (dbo.[CommunityRatings].[Rating]) 
	FROM dbo.[CommunityRatings] 
	WHERE dbo.[CommunityRatings].CommunityID = @CommunityID

	RETURN @AverageRating
END
