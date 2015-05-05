-------------------------------------------------------
-- 
-- Title            : Create of Table CommunityRatings
-- Description		: Create of Table CommunityRatings
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[CommunityRatings] (
    [CommunityRatingsID] BIGINT IDENTITY(1,1)	NOT NULL,
    [CommunityID]        BIGINT					NOT NULL,
    [Rating]             DECIMAL				NOT NULL,
    [RatedByID]          BIGINT					NULL,
    [CreatedDatetime]    DATETIME				NOT NULL,
    [ModifiedDatetime]   DATETIME				NULL,
    CONSTRAINT [CommunityRatings_PK] PRIMARY KEY CLUSTERED ([CommunityRatingsID] ASC),
    CONSTRAINT [Community_CommunityRatings_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_CommunityRatings_FK1] FOREIGN KEY ([RatedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

