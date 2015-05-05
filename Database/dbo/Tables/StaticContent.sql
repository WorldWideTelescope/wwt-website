-------------------------------------------------------
-- Title            : Create of Table StaticContent
-- Description      : Create of Table StaticContent
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-pladdh
-- Date             : 15 Dec 2011
-------------------------------------------------------
CREATE TABLE [dbo].[StaticContent]
(
    [StaticContentID]  BIGINT IDENTITY(1,1) NOT NULL,
    [TypeID]           INT                  NOT NULL,
    [Content]          NVARCHAR(MAX)        NOT NULL,
    [IsDeleted]        BIT                  NULL,
    [CreatedByID]      BIGINT               NOT NULL,
    [CreatedDatetime]  DATETIME             NULL,
    [ModifiedByID]     BIGINT               NULL,
    [ModifiedDatetime] DATETIME             NULL,
    [DeletedByID]      BIGINT               NULL,
    [DeletedDatetime]  DATETIME             NULL,
    CONSTRAINT [StaticContent_PK] PRIMARY KEY CLUSTERED ([StaticContentID] ASC),
    CONSTRAINT [StaticContentType_StaticContent_FK1] FOREIGN KEY ([TypeID]) REFERENCES [dbo].[StaticContentType] ([TypeID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_StaticContent_FK2] FOREIGN KEY ([CreatedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_StaticContent_FK3] FOREIGN KEY ([ModifiedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [User_StaticContent_FK4] FOREIGN KEY ([DeletedByID]) REFERENCES [dbo].[User] ([UserID]) ON DELETE NO ACTION ON UPDATE NO ACTION
)
