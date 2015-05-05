-------------------------------------------------------
-- 
-- Title            : Create of Table CommunityContents
-- Description		: Create of Table CommunityContents
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-------------------------------------------------------
CREATE TABLE [dbo].[CommunityContents] (
    [CommunityID]       BIGINT NOT NULL,
    [ContentID]      BIGINT NOT NULL,
    [SequenceNumber] INT              NULL,
    CONSTRAINT [Content_CommunityToContentRelation_FK2] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Community_CommunityToContentRelation_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION, 
    CONSTRAINT [PK_CommunityContents] PRIMARY KEY ([CommunityID], [ContentID])
);