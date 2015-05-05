-------------------------------------------------------
-- 
-- Title            : Create of Table OffenceType
-- Description		: Create of Table OffenceType
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 21 Sep 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[OffenceType] (
    [OffenceTypeID]	INT             	NOT NULL,
    [Name]			VARCHAR (250)		NOT NULL,
    CONSTRAINT [OffenceType_PK] PRIMARY KEY CLUSTERED ([OffenceTypeID] ASC)
);

