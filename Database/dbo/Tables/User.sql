-------------------------------------------------------
-- 
-- Title            : Create of Table User
-- Description		: Create of Table User
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[User] (
    [UserID]            BIGINT IDENTITY(1,1)    NOT NULL,
    [Email]             VARCHAR(1000)           NOT NULL,
    [LiveID]            VARCHAR(50)             NOT NULL,
    [FirstName]         NVARCHAR(50)            NOT NULL,
    [LastName]          NVARCHAR(50)            NOT NULL,
    [LastLoginDatetime] DATETIME                NULL,
    [IsDeleted]         BIT                     NULL,
    [DeletedByID]       BIGINT                  NULL,
    [UserTypeID]        INT                     NULL,
    [ConsumedSize]      DECIMAL                 NULL,
    [Affiliation]       NVARCHAR(300)           NULL, 
    [AboutMe]           NVARCHAR(MAX)          NULL, 
    [IsSubscribed]      BIT                     NOT NULL DEFAULT 1, 
    [PictureID] UNIQUEIDENTIFIER NULL, 
    [JoinedDateTime] DATETIME NULL, 
    CONSTRAINT [User_PK] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [User_User_FK1] FOREIGN KEY ([DeletedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [UserType_User_FK1] FOREIGN KEY ([UserTypeID]) REFERENCES [dbo].[UserType] ([UserTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);