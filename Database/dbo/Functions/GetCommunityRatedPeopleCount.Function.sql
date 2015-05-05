-------------------------------------------------------
-- 
-- Title            : [dbo].[GetCommunityRatedPeopleCount]
-- Description      : This function can be used to retrieve number of people rated for a Community.
-- Input            : ID of the community.
-- Output           : Number of people rated for the community.
-- Author           : v-sagna
-- Date             : 16 Aug 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].GetCommunityRatedPeopleCount
(
    @CommunityID BIGINT
)
RETURNS INT
AS
BEGIN
    DECLARE @RatedPeople INT

    SELECT @RatedPeople=COUNT(1)
    FROM dbo.[CommunityRatings] 
    WHERE dbo.[CommunityRatings].CommunityID = @CommunityID

    RETURN @RatedPeople
END