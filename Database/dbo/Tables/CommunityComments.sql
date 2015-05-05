-------------------------------------------------------
-- 
-- Title            : Create of Table CommunityComments
-- Description		: Create of Table CommunityComments
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[CommunityComments] (
    [CommunityCommentsID] BIGINT IDENTITY(1,1)	NOT NULL,
    [CommunityID]         BIGINT				NOT NULL,
    [Comment]             NVARCHAR(2000)		NOT NULL,
    [CommentedByID]       BIGINT				NOT NULL,
    [CommentedDatetime]   DATETIME				NOT NULL,
    [ReplyCommentID]      BIGINT				NULL,
    [IsDeleted]           BIT					NULL,
    CONSTRAINT [CommunityComments_PK] PRIMARY KEY CLUSTERED ([CommunityCommentsID] ASC),
    CONSTRAINT [CommunityComments_CommunityComments_FK1] FOREIGN KEY ([ReplyCommentID]) REFERENCES [dbo].[CommunityComments] ([CommunityCommentsID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Community_CommunityComments_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_CommunityComments_FK1] FOREIGN KEY ([CommentedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

