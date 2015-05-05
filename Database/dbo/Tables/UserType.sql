-------------------------------------------------------
-- 
-- Title            : Create of Table UserType
-- Description		: Create of Table UserType
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[UserType] (
    [UserTypeID]     INT IDENTITY(1,1) NOT NULL,
    [Name]           VARCHAR (250)  NOT NULL,
    [Description]    VARCHAR (1000) NULL,
    [MaxAllowedSize] DECIMAL            NULL,
    CONSTRAINT [UserType_PK] PRIMARY KEY CLUSTERED ([UserTypeID] ASC)
);

