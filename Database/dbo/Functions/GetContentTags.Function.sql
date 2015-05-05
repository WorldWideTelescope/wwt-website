-------------------------------------------------------
-- 
-- Title            : [dbo].[GetContentTags]
-- Description		: This function can be used to retrieve tags of a content
-- Input            : ID of the Content.
-- Output			: All tags of the content separated by comma.
-- Author			: v-vichan
-- Date             : 09 Aug 2011
-- 
-------------------------------------------------------
CREATE FUNCTION [dbo].[GetContentTags]
(
	@ContentID BIGINT
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @Tags NVARCHAR(4000)

	SELECT	@Tags = coalesce(@Tags + ',' , '') + dbo.[Tag].[Name] 
	FROM	dbo.[Content]
			LEFT OUTER JOIN dbo.[ContentTags]
				ON dbo.[ContentTags].[ContentID] = dbo.[Content].[ContentID]
			LEFT OUTER JOIN dbo.[Tag]
				ON dbo.[Tag].[TagID] = dbo.[ContentTags].[TagID]
	WHERE dbo.[Content].[ContentID] = @ContentID

	RETURN @Tags
END
GO
