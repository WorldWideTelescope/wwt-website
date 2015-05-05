-------------------------------------------------------
-- 
-- Title            : [dbo].[InviteRequestsView]
-- Description      : View for retrieving Invite Requests
-- Input            : N/A.
-- Output           : N/A.
-- Author           : v-sagna
-- Date             : 13 Dec 2011
-- 
-------------------------------------------------------
CREATE VIEW [dbo].[InviteRequestsView]
AS
SELECT  [dbo].[InviteRequest].[InviteRequestID]
        , [dbo].[InviteRequest].[InviteRequestToken]
        , [dbo].[InviteRequest].[EmailID]
        , [dbo].[InviteRequestContent].[RoleID]
        , [dbo].[InviteRequestContent].[Subject]
        , [dbo].[InviteRequestContent].[Body]
        , [dbo].[User].[FirstName] + ' ' + [dbo].[User].[LastName] AS [InvitedBy]
        , [dbo].[InviteRequestContent].[InvitedDate]
        , [dbo].[Community].[CommunityID]
        , [dbo].[Community].[Name] AS [CommunityName]
        , [dbo].[InviteRequest].[Used]
FROM    [dbo].[InviteRequest] 
        INNER JOIN [dbo].[InviteRequestContent] 
            ON [dbo].[InviteRequest].[InviteRequestContentID] = [dbo].[InviteRequestContent].[InviteRequestContentID]
        INNER JOIN [dbo].[Community]
            ON [dbo].[InviteRequestContent].[CommunityID] = [dbo].[Community].[CommunityID]
        INNER JOIN [dbo].[User]
            ON [dbo].[User].[UserID] = [dbo].[InviteRequestContent].[InvitedByID]
WHERE   [dbo].[InviteRequest].[IsDeleted] = 0