-------------------------------------------------------
-- 
-- Title            : Create of Table OffensiveCommunities
-- Description		: Create of Table OffensiveCommunities
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[OffensiveCommunities] (
    [OffensiveCommunitiesID] BIGINT IDENTITY(1,1)   NOT NULL,
    [OffensiveStatusID]      INT                    NOT NULL,
    [CommunityID]            BIGINT                 NOT NULL,
    [ReportedByID]           BIGINT                 NOT NULL,
    [ReportedDatetime]       DATETIME               NOT NULL,
    [Justification]          NVARCHAR(1050)         NULL,
    [ReviewerID]             BIGINT                 NULL,
    [ReviewerDatetime]       DATETIME               NULL,
    [Comments]               NVARCHAR(1050)         NULL, 
    [OffensiveTypeID]        INT                    NOT NULL, 
    CONSTRAINT [OffensiveCommunities_PK] PRIMARY KEY CLUSTERED ([OffensiveCommunitiesID] ASC),
    CONSTRAINT [Community_OffensiveCommunities_FK1] FOREIGN KEY ([CommunityID]) REFERENCES [dbo].[Community] ([CommunityID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [OffensiveStatus_OffensiveCommunities_FK1] FOREIGN KEY ([OffensiveStatusID]) REFERENCES [dbo].[OffensiveStatus] ([OffensiveStatusID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [OffensiveType_OffensiveCommunities_FK1] FOREIGN KEY ([OffensiveTypeID]) REFERENCES [dbo].[OffenceType] ([OffenceTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_OffensiveCommunities_FK1] FOREIGN KEY ([ReportedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_OffensiveCommunities_FK2] FOREIGN KEY ([ReviewerID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

