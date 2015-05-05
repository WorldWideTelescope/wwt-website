-------------------------------------------------------
-- 
-- Title            : Create of Table ContentRelationshipType
-- Description		: Create of Table ContentRelationshipType
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[ContentRelationshipType] (
    [ContentRelationshipTypeID] INT           NOT NULL,
    [Name]                      VARCHAR (250) NOT NULL,
    CONSTRAINT [PK_ContentRelationshipType] PRIMARY KEY CLUSTERED ([ContentRelationshipTypeID] ASC)
);

