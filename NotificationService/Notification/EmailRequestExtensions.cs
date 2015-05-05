//-----------------------------------------------------------------------
// <copyright file="EmailRequestExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WWTMVC5;
using WWTMVC5.Extensions;
using WWTMVC5.Models;
using WWTMVC5.Repositories.Interfaces;

namespace Microsoft.Research.EarthOnline.NotificationService.Notification
{
    /// <summary>
    /// Class having the extension methods needed for EmailRequest.
    /// </summary>
    public static class EmailRequestExtensions
    {
        public static EmailRequest UpdateFrom(this EmailRequest thisObject, object request)
        {
            //// TODO: Need to Get the Email request from the content of the message.
            //// TODO: Also we need to write a static methods for converting from the input request to the Email Request.

            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            JoinCommunityRequest joinRequest = request as JoinCommunityRequest;
            if (joinRequest != null)
            {
                return thisObject.UpdateFrom(joinRequest);
            }

            FlaggedRequest flaggedRequest = request as FlaggedRequest;
            if (flaggedRequest != null)
            {
                return thisObject.UpdateFrom(flaggedRequest);
            }

            EntityCommentRequest entityCommentRequest = request as EntityCommentRequest;
            if (entityCommentRequest != null)
            {
                return thisObject.UpdateFrom(entityCommentRequest);
            }

            ModeratorPermissionStatusRequest moderatorPermissionStatusRequest = request as ModeratorPermissionStatusRequest;
            if (moderatorPermissionStatusRequest != null)
            {
                return thisObject.UpdateFrom(moderatorPermissionStatusRequest);
            }

            UserPermissionStatusRequest userPermissionStatusRequest = request as UserPermissionStatusRequest;
            if (userPermissionStatusRequest != null)
            {
                return thisObject.UpdateFrom(userPermissionStatusRequest);
            }

            RemoveUserRequest removeUserRequest = request as RemoveUserRequest;
            if (removeUserRequest != null)
            {
                return thisObject.UpdateFrom(removeUserRequest);
            }

            UserPermissionChangedRequest userPermissionChangedRequest = request as UserPermissionChangedRequest;
            if (userPermissionChangedRequest != null)
            {
                return thisObject.UpdateFrom(userPermissionChangedRequest);
            }

            NotifyInviteRequest notifyInviteRequest = request as NotifyInviteRequest;
            if (notifyInviteRequest != null)
            {
                return thisObject.UpdateFrom(notifyInviteRequest);
            }

            EntityAdminActionRequest entityAdminDeleteRequest = request as EntityAdminActionRequest;
            if (entityAdminDeleteRequest != null)
            {
                return thisObject.UpdateFrom(entityAdminDeleteRequest);
            }

            NewEntityRequest newEntityRequest = request as NewEntityRequest;
            if (newEntityRequest != null)
            {
                return thisObject.UpdateFrom(newEntityRequest);
            }
            
            return null;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, JoinCommunityRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            IEnumerable<User> approvers = communityRepository.GetApprovers(request.CommunityID);

            foreach (User user in approvers)
            {
                if (user.IsSubscribed)
                {
                    thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));
                }
            }

            string entityName = request.CommunityName;
            bool isFolder = false;
            Community community = communityRepository.GetItem(c => c.CommunityID == request.CommunityID);
            if (community != null)
            {
                entityName = community.Name;
                isFolder = community.CommunityTypeID == (int)CommunityTypes.Folder;
            }

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;
            User requestor = userRepository.GetItem(u => u.UserID == request.RequestorID);

            thisObject.IsHtml = true;

            // Update the body and the subject.
            thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "{0} has requested to join your Layerscape {1}: \"{2}\"", requestor.GetFullName(), isFolder ? "folder" : "community", entityName);

            var replacements = new Dictionary<string, string>
            {
                { "@@ApproverName@@", string.Empty }, 
                { "@@CommunityName@@", HttpUtility.UrlDecode(entityName) }, 
                { "@@CommunityLink@@", HttpUtility.UrlDecode(request.CommunityLink) },
                { "@@RequestorName@@", HttpUtility.UrlDecode(requestor.GetFullName()) },
                { "@@RequestorID@@", HttpUtility.UrlDecode(request.RequestorID.ToString()) },
                { "@@RequestorLink@@", HttpUtility.UrlDecode(request.RequestorLink) },
                { "@@CommunityType@@", isFolder ? "folder" : "community" },
                { "@@PermissionRequested@@", HttpUtility.UrlDecode(request.PermissionRequested) },
            };
            thisObject.MessageBody = FormatMailBodyUsingTemplate("joincommunityrequest.html", replacements);

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, FlaggedRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            IContentRepository contentRepository = DependencyResolver.Current.GetService(typeof(IContentRepository)) as IContentRepository;

            IEnumerable<User> approvers = new List<User>();

            string entityName = string.Empty;
            if (request.EntityType == EntityType.Content)
            {
                Content content = contentRepository.GetItem(c => c.ContentID == request.ID);
                if (content != null)
                {
                    if (content.CommunityContents.Count > 0)
                    {
                        approvers = communityRepository.GetApprovers(Enumerable.ElementAt<CommunityContents>(content.CommunityContents, 0).CommunityID);
                    }

                    approvers.Concat(new[] { content.User });
                    entityName = content.Title;
                }
            }
            else
            {
                approvers = communityRepository.GetApprovers(request.ID);
                Community community = communityRepository.GetItem(c => c.CommunityID == request.ID);
                if (community != null)
                {
                    entityName = community.Name;
                }
            }

            foreach (User user in approvers)
            {
                if (user.IsSubscribed)
                {
                    thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));
                }
            }

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;
            User requestor = userRepository.GetItem(u => u.UserID == request.UserID);

            thisObject.IsHtml = true;

            // Update the body and the subject.
            thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "The Layerscape {0} \"{1}\" has been flagged by a user", request.EntityType.ToString().ToLower(), entityName);

            var replacements = new Dictionary<string, string>
            {
                { "@@ApproverName@@", string.Empty }, 
                { "@@Type@@", HttpUtility.UrlDecode(request.EntityType.ToString().ToLower()) },
                { "@@Name@@", HttpUtility.UrlDecode(entityName) }, 
                { "@@Link@@", HttpUtility.UrlDecode(request.Link) },
                { "@@UserName@@", HttpUtility.UrlDecode(requestor.GetFullName()) },
                { "@@UserLink@@", HttpUtility.UrlDecode(request.UserLink) },
                { "@@FlaggedOn@@", HttpUtility.UrlDecode(request.FlaggedOn.ToString()) },
                { "@@FlaggedAs@@", HttpUtility.UrlDecode(request.FlaggedAs) },
                { "@@UserComments@@", HttpUtility.UrlDecode(request.UserComments) },
            };
            thisObject.MessageBody = FormatMailBodyUsingTemplate("flaggedrequest.html", replacements);

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, EntityCommentRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            IContentRepository contentRepository = DependencyResolver.Current.GetService(typeof(IContentRepository)) as IContentRepository;

            IEnumerable<User> contributors = new List<User>();
            IEnumerable<User> commenters = new List<User>();

            string entityName = string.Empty;
            if (request.EntityType == EntityType.Content)
            {
                Content content = contentRepository.GetItem(c => c.ContentID == request.ID);
                if (content != null)
                {
                    if (content.CommunityContents.Count > 0)
                    {
                        contributors = communityRepository.GetContributors(Enumerable.ElementAt<CommunityContents>(content.CommunityContents, 0).CommunityID);
                    }

                    contributors.Concat(new[] { content.User });
                    entityName = content.Title;
                }

                IContentCommentsRepository contentCommentsRepository = DependencyResolver.Current.GetService(typeof(IContentCommentsRepository)) as IContentCommentsRepository;
                commenters = contentCommentsRepository.GetCommenters(request.ID);
            }
            else
            {
                contributors = communityRepository.GetContributors(request.ID);
                Community community = communityRepository.GetItem(c => c.CommunityID == request.ID);
                if (community != null)
                {
                    entityName = community.Name;
                }

                ICommunityCommentRepository communityCommentRepository = DependencyResolver.Current.GetService(typeof(ICommunityCommentRepository)) as ICommunityCommentRepository;
                commenters = communityCommentRepository.GetCommenters(request.ID);
            }

            List<User> recipients = new List<User>();
            if (contributors != null && contributors.Count() > 0)
            {
                recipients.AddRange(contributors);
            }

            if (commenters != null && commenters.Count() > 0)
            {
                recipients.AddRange(commenters.Where(p => !contributors.Any(x => x.UserID.Equals(p.UserID))));
            }

            foreach (User user in recipients)
            {
                if (user.IsSubscribed && user.UserID != request.UserID)
                {
                    thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));
                }
            }

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;
            User requestor = userRepository.GetItem(u => u.UserID == request.UserID);

            thisObject.IsHtml = true;

            // Update the body and the subject.
            thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "{0} has commented on the Layerscape {1} \"{2}\"", requestor.GetFullName(), request.EntityType.ToString().ToLower(), entityName);

            var replacements = new Dictionary<string, string>
            {
                { "@@RecipientName@@", string.Empty },
                { "@@Type@@", HttpUtility.UrlDecode(request.EntityType.ToString().ToLower()) },
                { "@@Name@@", HttpUtility.UrlDecode(entityName) }, 
                { "@@Link@@", HttpUtility.UrlDecode(request.Link) },
                { "@@UserName@@", HttpUtility.UrlDecode(requestor.GetFullName()) },
                { "@@UserLink@@", HttpUtility.UrlDecode(request.UserLink) },
                { "@@UserComments@@", HttpUtility.UrlDecode(request.UserComments) },
            };
            thisObject.MessageBody = FormatMailBodyUsingTemplate("entitycommentrequest.html", replacements);

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, ModeratorPermissionStatusRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            IContentRepository contentRepository = DependencyResolver.Current.GetService(typeof(IContentRepository)) as IContentRepository;

            IEnumerable<User> approvers = new List<User>();

            string entityName = string.Empty;
            bool isFolder = false;

            approvers = communityRepository.GetApprovers(request.CommunityID);
            Community community = communityRepository.GetItem(c => c.CommunityID == request.CommunityID);
            if (community != null)
            {
                entityName = community.Name;
                isFolder = community.CommunityTypeID == (int)CommunityTypes.Folder;
            }

            foreach (User user in approvers)
            {
                if (user.IsSubscribed && !(request.ApprovedRole >= UserRole.Moderator && request.RequestorID == user.UserID))
                {
                    thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));
                }
            }

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;
            User requestor = userRepository.GetItem(u => u.UserID == request.RequestorID);

            thisObject.IsHtml = true;

            // Update the body and the subject.
            thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "Request from {0} to join the Layerscape {1} {2} has been {3}", requestor.GetFullName(), isFolder ? "folder" : "community", entityName, request.IsApproved ? "Approved" : "Denied");

            var replacements = new Dictionary<string, string>
            {
                { "@@ApproverName@@", string.Empty }, 
                { "@@CommunityName@@", HttpUtility.UrlDecode(entityName) }, 
                { "@@CommunityLink@@", HttpUtility.UrlDecode(request.CommunityLink) },
                { "@@RequestorName@@", HttpUtility.UrlDecode(requestor.GetFullName()) },
                { "@@RequestorID@@", request.RequestorID.ToString() },
                { "@@RequestorLink@@", HttpUtility.UrlDecode(request.RequestorLink) },
                { "@@CommunityType@@", isFolder ? "folder" : "community" },
                { "@@PermissionStatus@@", HttpUtility.UrlDecode(request.IsApproved ? "Approved" : "Denied") },
            };
            thisObject.MessageBody = FormatMailBodyUsingTemplate("moderatorsapprovedrequest.html", replacements);

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, UserPermissionStatusRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            string entityName = string.Empty;
            bool isFolder = false;

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            Community community = communityRepository.GetItem(c => c.CommunityID == request.CommunityID);
            if (community != null)
            {
                entityName = community.Name;
                isFolder = community.CommunityTypeID == (int)CommunityTypes.Folder;
            }

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;
            User user = userRepository.GetItem(u => u.UserID == request.RequestorID);
            if (user.IsSubscribed)
            {
                thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));

                thisObject.IsHtml = true;

                // Update the body and the subject.
                thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "Your request to join the Layerscape {0} \"{1}\" has been {2}", isFolder ? "folder" : "community", entityName, request.IsApproved ? "Approved" : "Denied");

                var replacements = new Dictionary<string, string>
                {
                    { "@@CommunityName@@", HttpUtility.UrlDecode(entityName) }, 
                    { "@@CommunityLink@@", HttpUtility.UrlDecode(request.CommunityLink) },
                    { "@@RequestorName@@", HttpUtility.UrlDecode(user.GetFullName()) },
                    { "@@RequestorID@@", HttpUtility.UrlDecode(request.RequestorID.ToString()) },
                    { "@@RequestorLink@@", HttpUtility.UrlDecode(request.RequestorLink) },
                    { "@@PermissionStatus@@", HttpUtility.UrlDecode(request.IsApproved ? "Approved" : "Denied") },
                    { "@@CommunityType@@", isFolder ? "folder" : "community" },
                    { "@@Greetings@@", HttpUtility.UrlDecode(request.IsApproved ? "Congratulations! Your" : "We regret to inform you that your") },
                };
                thisObject.MessageBody = FormatMailBodyUsingTemplate("requestorapprovedrequest.html", replacements);
            }

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, RemoveUserRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            string entityName = string.Empty;
            bool isFolder = false;

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            Community community = communityRepository.GetItem(c => c.CommunityID == request.CommunityID);
            if (community != null)
            {
                entityName = community.Name;
                isFolder = community.CommunityTypeID == (int)CommunityTypes.Folder;
            }

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;
            User user = userRepository.GetItem(u => u.UserID == request.UserID);
            if (user.IsSubscribed)
            {
                thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));

                thisObject.IsHtml = true;

                // Update the body and the subject.
                thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "You no longer have permissions on the Layerscape {0} \"{1}\"", isFolder ? "folder" : "community", entityName);

                var replacements = new Dictionary<string, string>
                {
                    { "@@CommunityName@@", HttpUtility.UrlDecode(entityName) }, 
                    { "@@CommunityLink@@", HttpUtility.UrlDecode(request.CommunityLink) },
                    { "@@UserName@@", HttpUtility.UrlDecode(user.GetFullName()) },
                    { "@@CommunityType@@", isFolder ? "folder" : "community" },
                    { "@@UserLink@@", HttpUtility.UrlDecode(request.UserLink) },
                };
                thisObject.MessageBody = FormatMailBodyUsingTemplate("removeuserrequest.html", replacements);
            }
            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, UserPermissionChangedRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            string entityName = string.Empty;
            bool isFolder = false;

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            Community community = communityRepository.GetItem(c => c.CommunityID == request.CommunityID);
            if (community != null)
            {
                entityName = community.Name;
                isFolder = community.CommunityTypeID == (int)CommunityTypes.Folder;
            }

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;
            User user = userRepository.GetItem(u => u.UserID == request.UserID);
            if (user.IsSubscribed)
            {
                User moderator = userRepository.GetItem(u => u.UserID == request.ModeratorID);
                thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));

                thisObject.IsHtml = true;

                // Update the body and the subject.
                thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "You are now a {0} on the Layerscape {1} \"{2}\"", request.Role.ToString(), isFolder ? "folder" : "community", entityName);

                var replacements = new Dictionary<string, string>
                {
                    { "@@CommunityName@@", HttpUtility.UrlDecode(entityName) }, 
                    { "@@CommunityLink@@", HttpUtility.UrlDecode(request.CommunityLink) },
                    { "@@UserName@@", HttpUtility.UrlDecode(user.GetFullName()) },
                    { "@@UserLink@@", HttpUtility.UrlDecode(request.UserLink) },
                    { "@@Role@@", HttpUtility.UrlDecode(request.Role.ToString()) },
                    { "@@CommunityType@@", isFolder ? "folder" : "community" },
                    { "@@ModeratorName@@", HttpUtility.UrlDecode(moderator.GetFullName()) },
                    { "@@ModeratorLink@@", HttpUtility.UrlDecode(request.ModeratorLink) }
                };
                thisObject.MessageBody = FormatMailBodyUsingTemplate("permissionchangerequest.html", replacements);
            }

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, NotifyInviteRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            thisObject.Recipients.Add(new MailAddress(request.EmailId));

            thisObject.IsHtml = true;

            // Update the body and the subject.
            thisObject.Subject = request.Subject;

            var replacements = new Dictionary<string, string>
            {
                { "@@Title@@", HttpUtility.UrlDecode(request.Subject) },
                { "@@Body@@", HttpUtility.UrlDecode(request.Body) },
                { "@@EntityName@@", HttpUtility.UrlDecode(request.CommunityName) },
                { "@@EntityLink@@", HttpUtility.UrlDecode(request.CommunityLink) },
                { "@@InviteLink@@", HttpUtility.UrlDecode(request.InviteLink) },
                { "@@ContactUsLink@@", string.Format(CultureInfo.CurrentUICulture, "mailto:{0}", Constants.MicrosoftEmail) },
                { "@@ForumsLink@@", Constants.WWTForumUrl }
            };

            thisObject.MessageBody = FormatMailBodyUsingTemplate("communityinviterequest.html", replacements);

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, EntityAdminActionRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            ICommunityRepository communityRepository = DependencyResolver.Current.GetService(typeof(ICommunityRepository)) as ICommunityRepository;
            IContentRepository contentRepository = DependencyResolver.Current.GetService(typeof(IContentRepository)) as IContentRepository;

            IEnumerable<User> approvers = new List<User>();

            string entityName = string.Empty;
            if (request.EntityType == EntityType.Content)
            {
                Content content = contentRepository.GetItem(c => c.ContentID == request.EntityID);
                if (content != null)
                {
                    if (content.CommunityContents.Count > 0)
                    {
                        approvers = communityRepository.GetApprovers(Enumerable.ElementAt<CommunityContents>(content.CommunityContents, 0).CommunityID);
                    }

                    approvers.Concat(new[] { content.User });
                    entityName = content.Title;
                }
            }
            else
            {
                approvers = communityRepository.GetApprovers(request.EntityID);
                Community community = communityRepository.GetItem(c => c.CommunityID == request.EntityID);
                if (community != null)
                {
                    entityName = community.Name;
                }
            }

            foreach (User user in approvers)
            {
                if (user.IsSubscribed)
                {
                    thisObject.Recipients.Add(new MailAddress(user.Email.FixEmailAddress(), user.FirstName + " " + user.LastName));
                }
            }

            thisObject.IsHtml = true;

            // Update the body and the subject.
            switch (request.Action)
            {
                case AdminActions.Delete:
                    thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "The Layerscape {0} \"{1}\" has been deleted by the site admin", request.EntityType.ToString().ToLower(), entityName);
                    break;
                case AdminActions.MarkAsPrivate:
                    thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "The Layerscape {0} \"{1}\" has been marked as Private by the site admin", request.EntityType.ToString().ToLower(), entityName);
                    break;
            }

            var replacements = new Dictionary<string, string>
            {
                { "@@UserName@@", string.Empty }, 
                { "@@EntityType@@", HttpUtility.UrlDecode(request.EntityType.ToString().ToLower()) },
                { "@@EntityName@@", HttpUtility.UrlDecode(entityName) }, 
                { "@@EntityLink@@", HttpUtility.UrlDecode(request.EntityLink) },
                { "@@ContactUsLink@@", string.Format(CultureInfo.CurrentUICulture, "mailto:{0}", Constants.MicrosoftEmail) }
            };
            thisObject.MessageBody = FormatMailBodyUsingTemplate(string.Format(CultureInfo.InvariantCulture, "entityadmin{0}request.html", request.Action.ToString().ToLower()), replacements);

            return thisObject;
        }

        public static EmailRequest UpdateFrom(this EmailRequest thisObject, NewEntityRequest request)
        {
            if (thisObject == null)
            {
                thisObject = new EmailRequest();
            }

            thisObject.Recipients.Add(new MailAddress(Constants.MicrosoftEmail));
            
            thisObject.IsHtml = true;

            IUserRepository userRepository = DependencyResolver.Current.GetService(typeof(IUserRepository)) as IUserRepository;

            switch (request.EntityType)
            {
                case EntityType.Community:
                case EntityType.Folder:
                case EntityType.Content:
                    {
                        User user = userRepository.GetItem(u => u.UserID == request.UserID);

                        // Update the body and the subject.
                        thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "New {0} \"{1}\" added to Layerscape", request.EntityType.ToString().ToLower(), request.EntityName);
                        var replacements = new Dictionary<string, string>
                        {
                            { "@@EntityType@@", HttpUtility.UrlDecode(request.EntityType.ToString().ToLower()) },
                            { "@@EntityName@@", HttpUtility.UrlDecode(request.EntityName) }, 
                            { "@@EntityLink@@", HttpUtility.UrlDecode(request.EntityLink) },
                            { "@@UserName@@", HttpUtility.UrlDecode(user.GetFullName()) },
                            { "@@UserLink@@", HttpUtility.UrlDecode(request.UserLink) }
                        };

                        thisObject.MessageBody = FormatMailBodyUsingTemplate("newentityrequest.html", replacements);
                        break;
                    }
                case EntityType.User:
                    {
                        User user = userRepository.GetItem(u => u.UserID == request.EntityID);

                        // Update the body and the subject.
                        thisObject.Subject = string.Format(CultureInfo.CurrentUICulture, "New {0} \"{1}\" added to Layerscape", request.EntityType.ToString().ToLower(), user.GetFullName());

                        var replacements = new Dictionary<string, string>
                        {
                            { "@@UserName@@", HttpUtility.UrlDecode(user.GetFullName()) },
                            { "@@UserLink@@", HttpUtility.UrlDecode(request.EntityLink) }
                        };

                        thisObject.MessageBody = FormatMailBodyUsingTemplate("newuserrequest.html", replacements);
                        break;
                    }
            }
            return thisObject;
        }

        /// <summary>
        /// Function used for formatting mail message. 
        /// Reads template from file, makes global search/replace operations, then returns the result string.
        /// </summary>
        /// <param name="templateFileName">Name of the template file. The name has to be relative to the current assemblies executing directory.</param>
        /// <param name="replacements">set or replacements. The function replaces all keys with corresponding values from this dictionary object.</param>
        /// <returns>Formatted body.</returns>
        private static string FormatMailBodyUsingTemplate(string templateFileName, Dictionary<string, string> replacements)
        {
            string templateFullName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates", templateFileName);

            string body = null;
            using (var reader = new StreamReader(templateFullName))
            {
                body = reader.ReadToEnd();
            }

            foreach (var r in replacements)
            {
                body = body.Replace(r.Key, HttpUtility.HtmlEncode(r.Value).Replace("\n", "<br />"));
            }

            return body;
        }
    }
}