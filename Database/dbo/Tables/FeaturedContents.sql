-------------------------------------------------------
-- 
-- Title            : Create of Table FeaturedContents
-- Description		: Create of Table FeaturedContents
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 22 Dec 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[FeaturedContents] (
    [FeaturedContentsID]    BIGINT IDENTITY(1,1)   NOT NULL,
    [ContentID]             BIGINT                 NOT NULL,
	[CategoryID]            INT					   NULL,
	[SortOrder]             INT					   NULL,
    [UpdatedByID]           BIGINT                 NULL,
    [UpdatedDatetime]       DATETIME               NULL,
    CONSTRAINT [FeaturedContents_PK] PRIMARY KEY CLUSTERED ([FeaturedContentsID] ASC),
    CONSTRAINT [Content_FeaturedContents_FK1] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Category_FeaturedContents_FK1] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_FeaturedContents_FK1] FOREIGN KEY ([UpdatedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);

