-------------------------------------------------------
-- 
-- Title            : InviteRequest
-- Description      : Table capturing the details about the Invite requests for a community sent by the community owner or moderator.
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-sagna
-- Date             : 13 Dec 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[InviteRequest] (
    [InviteRequestID]           INT IDENTITY(1,1)   NOT NULL,
    [InviteRequestToken]        UNIQUEIDENTIFIER    NOT NULL,
    [EmailID]                   NVARCHAR(1000)      NOT NULL,
    [Used]                      BIT                 NULL,
    [UsedByID]                  BIGINT              NULL,
    [UsedDate]                  DATETIME            NULL,
    [IsDeleted]                 BIT                 NULL,
    [DeletedByID]               BIGINT              NULL,
    [DeletedDate]               DATETIME            NULL,
    [InviteRequestContentID]    INT                 NOT NULL,
    CONSTRAINT [InviteRequest_PK] PRIMARY KEY CLUSTERED ([InviteRequestID] ASC),
    CONSTRAINT [InviteRequest_User_FK1] FOREIGN KEY ([UsedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [InviteRequest_User_FK2] FOREIGN KEY ([DeletedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [InviteRequest_InviteRequestContent_FK1] FOREIGN KEY ([InviteRequestContentID]) REFERENCES [dbo].[InviteRequestContent] ([InviteRequestContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);