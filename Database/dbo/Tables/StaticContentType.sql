-------------------------------------------------------
-- Title            : Create of Table StaticContentType
-- Description      : Create of Table StaticContentType
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-pladdh
-- Date             : 15 Dec 2011
-------------------------------------------------------
CREATE TABLE [dbo].[StaticContentType]
(
    [TypeID]    INT IDENTITY(1,1)    NOT NULL,
    [Name]        VARCHAR (250)    NOT NULL,
    [Description]    VARCHAR (1000)    NOT NULL,
    CONSTRAINT [StaticContentType_PK] PRIMARY KEY CLUSTERED ([TypeID] ASC)
)
