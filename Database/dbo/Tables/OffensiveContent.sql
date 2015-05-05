-------------------------------------------------------
-- 
-- Title            : Create of Table OffensiveContent
-- Description		: Create of Table OffensiveContent
-- Input            : N/A.
-- Output			: N/A.
-- Author			: v-vichan
-- Date             : 08 Aug 2011
-- 
-------------------------------------------------------
CREATE TABLE [dbo].[OffensiveContent] (
    [OffensiveContentID] BIGINT IDENTITY(1,1)   NOT NULL,
    [OffensiveStatusID]  INT                    NOT NULL,
    [ContentID]          BIGINT                 NOT NULL,
    [ReportedByID]       BIGINT                 NOT NULL,
    [ReportedDatetime]   DATETIME               NOT NULL,
    [Justification]      NVARCHAR(1050)         NULL,
    [ReviewerID]         BIGINT                 NULL,
    [ReviewerDatetime]   DATETIME               NULL,
    [Comments]           NVARCHAR(1050)         NULL, 
    [OffensiveTypeID]    INT                    NOT NULL, 
    CONSTRAINT [OffensiveContent_PK] PRIMARY KEY CLUSTERED ([OffensiveContentID] ASC),
    CONSTRAINT [Content_OffensiveContent_FK1] FOREIGN KEY ([ContentID]) REFERENCES [dbo].[Content] ([ContentID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [OffensiveStatus_OffensiveContent_FK1] FOREIGN KEY ([OffensiveStatusID]) REFERENCES [dbo].[OffensiveStatus] ([OffensiveStatusID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [OffensiveType_OffensiveContent_FK1] FOREIGN KEY ([OffensiveTypeID]) REFERENCES [dbo].[OffenceType] ([OffenceTypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_OffensiveContent_FK1] FOREIGN KEY ([ReportedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_OffensiveContent_FK2] FOREIGN KEY ([ReviewerID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

