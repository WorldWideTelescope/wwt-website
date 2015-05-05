//-----------------------------------------------------------------------
// <copyright file="DataDetailExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for AssociatedFileDetail.
    /// </summary>
    public static class DataDetailExtensions
    {
        /// <summary>
        /// Populates the FileDetail object's properties from the given Content object's properties.
        /// </summary>
        /// <param name="thisObject">Current FileDetail instance on which the extension method is called</param>
        /// <param name="content">Content model from which values to be read</param>
        public static DataDetail SetValuesFrom(this DataDetail thisObject, Content content)
        {
            if (content != null)
            {
                // Set Content Type.
                ContentTypes type = content.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);

                if (type == ContentTypes.Link)
                {
                    thisObject = new LinkDetail(content.ContentUrl, content.ContentID);
                }
                else
                {
                    var fileDetail = new FileDetail();
                    fileDetail.SetValuesFrom(content);
                    thisObject = fileDetail;
                }

                thisObject.Name = content.Filename;
                thisObject.ContentType = content.TypeID.ToEnum<int, ContentTypes>(ContentTypes.Generic);
                thisObject.ContentType = type;
            }

            return thisObject;
        }
    }
}