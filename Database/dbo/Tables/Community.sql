-------------------------------------------------------
-- 
-- Title            : Create of Table Community
-- Description		: Create of Table Community
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[Community] (
	[CommunityID]      BIGINT IDENTITY(1,1) NOT NULL,
	[Name]             NVARCHAR(250)    NOT NULL,
	[Description]      NVARCHAR(MAX)   NULL,
	[AccessTypeID]     INT              NULL,
	[CategoryID]       INT					NOT NULL,
	[DistributedBy]    NVARCHAR(MAX)			NULL,
	[ThumbnailID]      UNIQUEIDENTIFIER		NULL,
	[IsDeleted]        BIT              NULL,
	[IsOffensive]      BIT              NULL,
	[CreatedByID]      BIGINT           NOT NULL,
	[CreatedDatetime]  DATETIME         NULL,
	[ModifiedByID]     BIGINT              NULL,
	[ModifiedDatetime] DATETIME         NULL,
	[DeletedByID]      BIGINT              NULL,
	[DeletedDatetime]  DATETIME         NULL,
	[CommunityTypeID]  INT NOT NULL, 
    [ViewCount] BIGINT NULL, 
    CONSTRAINT [Community_PK] PRIMARY KEY CLUSTERED ([CommunityID] ASC),
	CONSTRAINT [AccessType_Community_FK1] FOREIGN KEY ([AccessTypeID]) REFERENCES [dbo].[AccessType] ([AccessTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [Category_Community_FK1] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [User_Community_FK1] FOREIGN KEY ([CreatedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [User_Community_FK2] FOREIGN KEY ([ModifiedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [CommunityType_Community_FK1] FOREIGN KEY ([CommunityTypeID]) REFERENCES [dbo].[CommunityType] ([CommunityTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION, 
	CONSTRAINT [User_Community_FK4] FOREIGN KEY ([DeletedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

