-------------------------------------------------------
-- 
-- Title            : Create of Table CommunityType
-- Description		: Create of Table CommunityType
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 10 Oct 2011
-------------------------------------------------------
CREATE TABLE [dbo].[CommunityType] (
    [CommunityTypeID]   INT          NOT NULL,
    [Name]              VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_CommunityType] PRIMARY KEY CLUSTERED ([CommunityTypeID] ASC)
);

