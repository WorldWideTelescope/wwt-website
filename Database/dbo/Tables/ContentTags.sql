-------------------------------------------------------
-- 
-- Title            : Create of Table ContentTags
-- Description		: Create of Table ContentTags
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[ContentTags] (
	[ContentID]     BIGINT NOT NULL,
	[TagID]         INT              NOT NULL,
	[TaggedByID]    BIGINT              NULL,
	CONSTRAINT [ContentTags_PK] PRIMARY KEY CLUSTERED ([ContentID], [TagID]),
	CONSTRAINT [Content_ContentTags_FK1] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [Tag_ContentTags_FK1] FOREIGN KEY ([TagID]) REFERENCES [dbo].[Tag] ([TagID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [User_ContentTags_FK1] FOREIGN KEY ([TaggedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

