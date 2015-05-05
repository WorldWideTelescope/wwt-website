-------------------------------------------------------
-- 
-- Title            : InviteRequestContent
-- Description      : Table capturing the details about the Invite requests contents for a community sent by the community owner or moderator.
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-sagna
-- Date             : 13 Dec 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[InviteRequestContent] (
    [InviteRequestContentID]    INT IDENTITY(1,1)   NOT NULL,
    [CommunityID]               BIGINT              NOT NULL,
    [RoleID]                    INT                 NOT NULL,
    [Subject]                   NVARCHAR(250)       NULL,
    [Body]                      NVARCHAR(2000)      NULL,
    [InvitedByID]               BIGINT              NULL,
    [InvitedDate]               DATETIME            NULL,
    CONSTRAINT [InviteRequestContent_PK] PRIMARY KEY CLUSTERED ([InviteRequestContentID] ASC),
    CONSTRAINT [InviteRequestContent_Community_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [InviteRequestContent_Role_FK1] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Role] ([RoleID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [InviteRequestContent_User_FK1] FOREIGN KEY ([InvitedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);