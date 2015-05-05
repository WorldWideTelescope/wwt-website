-------------------------------------------------------
-- 
-- Title            : Create of Table OffensiveStatus
-- Description		: Create of Table OffensiveStatus
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[OffensiveStatus] (
    [OffensiveStatusID] INT           NOT NULL,
    [Type]              NVARCHAR(250) NULL,
    CONSTRAINT [OffensiveStatus_PK] PRIMARY KEY CLUSTERED ([OffensiveStatusID] ASC)
);

