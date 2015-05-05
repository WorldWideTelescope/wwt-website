-------------------------------------------------------
-- 
-- Title            : Create of Table ContentRelation
-- Description		: Create of Table ContentRelation
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[ContentRelation] (
    [ContentID]                 BIGINT NOT NULL,
    [RelatedContentID]          BIGINT NOT NULL,
    [ContentRelationshipTypeID] INT              NOT NULL,
    [SequenceNumber]            INT              NOT NULL,
    CONSTRAINT [Content_ContentRelation_FK1] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Content_ContentRelation_FK2] FOREIGN KEY ([RelatedContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_ContentRelation_ContentRelationshipType] FOREIGN KEY ([ContentRelationshipTypeID]) REFERENCES [dbo].[ContentRelationshipType] ([ContentRelationshipTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION, 
    CONSTRAINT [PK_ContentRelation] PRIMARY KEY ([ContentID], [RelatedContentID], [ContentRelationshipTypeID])
);

