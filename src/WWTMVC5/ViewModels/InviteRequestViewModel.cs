//-----------------------------------------------------------------------
// <copyright file="InviteRequestViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the Invite Requests of a Community
    /// </summary>
    public class InviteRequestViewModel
    {
        /// <summary>
        /// Initializes a new instance of the InviteRequestViewModel class.
        /// </summary>
        public InviteRequestViewModel(IList<InviteRequestDetailsViewModel> inviteRequestList, PageDetails paginationDetails)
        {
            this.InviteRequestList = inviteRequestList;
            this.PaginationDetails = paginationDetails;
        }

        /// <summary>
        /// Gets the Invite Requests list of a community
        /// </summary>
        public IList<InviteRequestDetailsViewModel> InviteRequestList { get; private set; }

        /// <summary>
        /// Gets or sets the page details for the Invite Requests.
        /// </summary>
        public PageDetails PaginationDetails { get; set; }
    }
}