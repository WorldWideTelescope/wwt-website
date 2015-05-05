-------------------------------------------------------
-- 
-- Title            : Create of Table FeaturedCommunities
-- Description		: Create of Table FeaturedCommunities
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 22 Dec 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[FeaturedCommunities] (
    [FeaturedCommunitiesID] BIGINT IDENTITY(1,1)   NOT NULL,
    [CommunityID]           BIGINT                 NOT NULL,
	[CategoryID]            INT					   NULL,
	[SortOrder]             INT					   NULL,
    [UpdatedByID]           BIGINT                 NULL,
    [UpdatedDatetime]       DATETIME               NULL,
    CONSTRAINT [FeaturedCommunities_PK] PRIMARY KEY CLUSTERED ([FeaturedCommunitiesID] ASC),
    CONSTRAINT [Community_FeaturedCommunities_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Category_FeaturedCommunities_FK1] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category] ([CategoryID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_FeaturedCommunities_FK1] FOREIGN KEY ([UpdatedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);

