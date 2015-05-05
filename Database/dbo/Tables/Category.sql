-------------------------------------------------------
-- 
-- Title            : Create of Table Category
-- Description      : Create of Table Category
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[Category] (
    [CategoryID]    INT             NOT NULL,
    [Name]          NVARCHAR(250)   NOT NULL,
    [Description]   NVARCHAR(1000)  NULL,
    [IsDeleted]     BIT             NOT NULL,
    CONSTRAINT  [Category_PK] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

