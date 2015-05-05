-------------------------------------------------------
-- 
-- Title            : [dbo].[GetAverageContentRating]
-- Description		: This function can be used to retrieve average rating of a Content
-- Input            : ID of the Content.
-- Output			: Average Rating of the Content.
-- Author			: v-vichan
-- Date             : 09 Aug 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].[GetAverageContentRating]
(
	@ContentID BIGINT
)
RETURNS DECIMAL(3,2)
AS
BEGIN
	DECLARE @AverageRating DECIMAL(3,2)

	SELECT @AverageRating = AVG (dbo.[ContentRatings].[Rating]) 
	FROM dbo.[ContentRatings] 
	WHERE dbo.[ContentRatings].[ContentID] = @ContentID

	RETURN @AverageRating
END