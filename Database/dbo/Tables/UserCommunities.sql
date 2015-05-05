-------------------------------------------------------
-- 
-- Title            : Create of Table UserCommunities
-- Description		: Create of Table UserCommunities
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[UserCommunities] (
	[UserID]            BIGINT		NOT NULL,
	[CommunityId]       BIGINT		NOT NULL,
	[RoleID]            INT         NOT NULL,
	[CreatedDatetime]   DATETIME    NULL,
    [IsInherited]       BIT         NOT NULL,
	CONSTRAINT [UserCommunities_PK] PRIMARY KEY CLUSTERED ([UserID], [CommunityId]),
	CONSTRAINT [Community_UserCommunities_FK1] FOREIGN KEY ([CommunityId]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [Role_UserCommunities_FK1] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Role] ([RoleID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT [User_UserCommunities_FK1] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

