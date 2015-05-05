-------------------------------------------------------
-- 
-- Title            : Create of Table ContentRatings
-- Description		: Create of Table ContentRatings
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[ContentRatings] (
    [ContentRatingsID] BIGINT IDENTITY(1,1) NOT NULL,
    [ContentID]        BIGINT				NOT NULL,
    [Rating]           DECIMAL              NOT NULL,
    [RatingByID]       BIGINT				NOT NULL,
    [CreatedDatetime]  DATETIME				NOT NULL,
    [ModifiedDatetime] DATETIME				NULL,
    CONSTRAINT [ContentRatings_PK] PRIMARY KEY CLUSTERED ([ContentRatingsID] ASC),
    CONSTRAINT [Content_ContentRatings_FK1] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_ContentRatings_FK1] FOREIGN KEY ([RatingByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

