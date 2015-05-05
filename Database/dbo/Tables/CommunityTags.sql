-------------------------------------------------------
-- 
-- Title            : Create of Table CommunityTags
-- Description		: Create of Table CommunityTags
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[CommunityTags] (
	[CommunityID]     BIGINT	NOT NULL,
	[TagID]           INT       NOT NULL,
	[TaggedByID]      BIGINT    NULL,
	CONSTRAINT [CommunityTags_PK] PRIMARY KEY CLUSTERED ([CommunityID], [TagID]),
	CONSTRAINT [Community_CommunityTags_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [Tag_CommunityTags_FK1] FOREIGN KEY ([TagID]) REFERENCES [dbo].[Tag] ([TagID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [User_CommunityTags_FK1] FOREIGN KEY ([TaggedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

