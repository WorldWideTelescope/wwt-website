//-----------------------------------------------------------------------
// <copyright file="VideoDataViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the view model for rendering the view for a Video details
    /// to be shown in Add Video section of publish content page.
    /// </summary>
    public class VideoDataViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the video
        /// </summary>
        public Guid VideoID { get; set; }

        /// <summary>
        /// Gets or sets Video name.
        /// </summary>
        public string VideoName { get; set; }

        /// <summary>
        /// Gets or sets the video details
        /// </summary>
        public string VideoFileDetail { get; set; }
    }
}