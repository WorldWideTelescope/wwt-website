-------------------------------------------------------
-- 
-- Title            : PermissionRequest
-- Description      : Table capturing the details about the permission requests for a community sent by users.
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-sagna
-- Date             : 13 Oct 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[PermissionRequest] (
    [PermissionRequestID]   INT IDENTITY(1,1)   NOT NULL,
    [UserID]                BIGINT              NOT NULL,
    [CommunityID]           BIGINT              NOT NULL,
    [RoleID]                INT                 NOT NULL,
    [Comment]               NVARCHAR(1050)      NULL,
    [RequestedDate]         DATETIME            NULL,
    [Approved]              BIT                 NULL,
    [RespondedByID]         BIGINT              NULL,
    [RespondedDate]         DATETIME            NULL,
    CONSTRAINT [PermissionRequest_PK] PRIMARY KEY CLUSTERED ([PermissionRequestID] ASC),
    CONSTRAINT [User_PermissionRequest_FK1] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Community_PermissionRequest_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [Role_PermissionRequest_FK1] FOREIGN KEY ([RoleID]) REFERENCES [dbo].[Role] ([RoleID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_PermissionRequest_FK2] FOREIGN KEY ([RespondedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
);