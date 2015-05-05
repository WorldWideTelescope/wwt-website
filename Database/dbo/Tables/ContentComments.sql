-------------------------------------------------------
-- 
-- Title            : Create of Table ContentComments
-- Description		: Create of Table ContentComments
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[ContentComments] (
    [ContentCommentsID] BIGINT IDENTITY(1,1) NOT NULL,
    [ContentID]         BIGINT NOT NULL,
    [Comment]           NVARCHAR(2000)    NOT NULL,
    [CommentedByID]     BIGINT              NULL,
    [CommentDatetime]   DATETIME         NULL,
    [ReplyCommentID]    BIGINT              NULL,
    [IsDeleted]         BIT              NULL,
    CONSTRAINT [ContentComments_PK] PRIMARY KEY CLUSTERED ([ContentCommentsID] ASC),
    CONSTRAINT [Content_ContentComments_FK1] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [ContentComments_ContentComments_FK1] FOREIGN KEY ([ReplyCommentID]) REFERENCES [dbo].[ContentComments] ([ContentCommentsID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_ContentComments_FK1] FOREIGN KEY ([CommentedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

