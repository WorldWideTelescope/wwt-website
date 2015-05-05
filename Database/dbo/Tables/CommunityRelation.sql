-------------------------------------------------------
-- 
-- Title            : Create of Table CommunityRelation
-- Description		: Create of Table CommunityRelation
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[CommunityRelation] (
    [ParentCommunityID] BIGINT NOT NULL,
    [ChildCommunityID]  BIGINT NOT NULL,
    [SequenceNumber] INT              NULL,
    CONSTRAINT [Community_CommunityRelation_FK1] FOREIGN KEY ([ParentCommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Community_CommunityRelation_FK2] FOREIGN KEY ([ChildCommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION, 
    CONSTRAINT [PK_CommunityRelation] PRIMARY KEY ([ParentCommunityID], [ChildCommunityID])
);

