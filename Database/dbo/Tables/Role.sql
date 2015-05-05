-------------------------------------------------------
-- 
-- Title            : Create of Table Role
-- Description		: Create of Table Role
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[Role] (
    [RoleID]      INT IDENTITY(1,1) NOT NULL,
    [Name]        VARCHAR (250)  NOT NULL,
    [Description] VARCHAR(1000) NULL,
    CONSTRAINT [Role_PK] PRIMARY KEY CLUSTERED ([RoleID] ASC)
);

