-------------------------------------------------------
-- 
-- Title            : Create of Table Content
-- Description      : Create of Table Content
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[Content] (
    [ContentID]        BIGINT IDENTITY(1,1) NOT NULL,
    [TypeID]           INT                  NOT NULL,
    [Title]            NVARCHAR(256)        NOT NULL,
    [Filename]         NVARCHAR(256)        NOT NULL,
    [Description]      NVARCHAR(MAX)       NULL,
    [ContentAzureID]   UNIQUEIDENTIFIER     NOT NULL,
    [ContentAzureURL]  NVARCHAR(250)        NULL,
    [Size]             DECIMAL              NULL,
    [DownloadCount]    INT                  NULL,
    [AccessTypeID]     INT                  NULL,
    [ThumbnailID]      UNIQUEIDENTIFIER     NULL, 
    [IsSearchable]     BIT                  NOT NULL,
    [IsOffensive]      BIT                  NULL,
    [IsDeleted]        BIT                  NULL,
    [CreatedByID]      BIGINT               NOT NULL,
    [CreatedDatetime]  DATETIME             NULL,
    [ModifiedByID]     BIGINT               NULL,
    [ModifiedDatetime] DATETIME             NULL,
    [DeletedByID]      BIGINT               NULL,
    [DeletedDatetime]  DATETIME             NULL,
    [CategoryID] INT NOT NULL, 
    [DistributedBy] NVARCHAR(MAX) NULL, 
    [Citation] NVARCHAR(MAX) NULL, 
    [ContentUrl] VARCHAR(2050) NULL, 
    [TourRunLength] VARCHAR(10) NULL, 
    CONSTRAINT [Content_PK] PRIMARY KEY CLUSTERED ([ContentID] ASC),
    CONSTRAINT [AccessType_Content_FK1] FOREIGN KEY ([AccessTypeID]) REFERENCES [dbo].[AccessType] ([AccessTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [ContentType_Content_FK1] FOREIGN KEY ([TypeID]) REFERENCES [dbo].[ContentType] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_Content_FK2] FOREIGN KEY ([CreatedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_Content_FK3] FOREIGN KEY ([ModifiedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_Content_FK4] FOREIGN KEY ([DeletedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION, 
    CONSTRAINT [Category_Content_FK1] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

