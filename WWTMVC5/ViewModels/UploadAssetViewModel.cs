//-----------------------------------------------------------------------
// <copyright file="UploadAssetViewModel.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using WWTMVC5.Models;

namespace WWTMVC5.ViewModels
{
    /// <summary>
    /// Class representing the model for rendering the asset files
    /// </summary>
    public class UploadAssetViewModel
    {
        /// <summary>
        /// Initializes a new instance of the UploadAssetViewModel class.
        /// </summary>
        public UploadAssetViewModel(IList<BlobDetails> assets, PageDetails paginationDetails)
        {
            this.Assets = assets;
            this.PaginationDetails = paginationDetails;
        }

        /// <summary>
        /// Gets the collection of assets
        /// </summary>
        public IList<BlobDetails> Assets { get; private set; }

        /// <summary>
        /// Gets or sets the page details for the Assets.
        /// </summary>
        public PageDetails PaginationDetails { get; set; }
    }
}