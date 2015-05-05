-------------------------------------------------------
-- 
-- Title            : Create of Table Tag
-- Description		: Create of Table Tag
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[Tag] (
    [TagID] INT IDENTITY(1,1) NOT NULL,
    [Name]  NVARCHAR(250) NOT NULL,
    CONSTRAINT [Tag_PK] PRIMARY KEY CLUSTERED ([TagID] ASC)
);

