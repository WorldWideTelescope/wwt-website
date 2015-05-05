-------------------------------------------------------
-- 
-- Title            : [dbo].[GetContentRatedPeopleCount]
-- Description      : This function can be used to retrieve number of people rated for a Content.
-- Input            : ID of the Content.
-- Output           : Number of people rated for the Content.
-- Author           : v-sagna
-- Date             : 16 Aug 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].GetContentRatedPeopleCount
(
    @ContentID BIGINT
)
RETURNS INT
AS
BEGIN
    DECLARE @RatedPeople INT

    SELECT @RatedPeople=COUNT(1)
    FROM dbo.[ContentRatings] 
    WHERE dbo.[ContentRatings].[ContentID] = @ContentID

    RETURN @RatedPeople
END