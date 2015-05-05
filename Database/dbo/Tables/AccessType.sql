-------------------------------------------------------
-- 
-- Title            : Create of Table AccessType
-- Description		: Create of Table AccessType
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[AccessType] (
    [AccessTypeID]	INT IDENTITY(1,1)	NOT NULL,
    [Name]			VARCHAR (250)		NOT NULL,
    [Description]	VARCHAR (1000)		NOT NULL,
    CONSTRAINT [AccessType_PK] PRIMARY KEY CLUSTERED ([AccessTypeID] ASC)
);

