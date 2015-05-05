-------------------------------------------------------
-- 
-- Title            : Create of Table ContentType
-- Description		: Create of Table ContentType
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[ContentType] (
    [ID]   INT NOT NULL,
    [Name] VARCHAR (250) NOT NULL,
    CONSTRAINT [ContentType_PK] PRIMARY KEY CLUSTERED ([ID] ASC)
);

