//-----------------------------------------------------------------------
// <copyright file="StreamExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Stream.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// This function is used to Generate the thumbnail of the input image stream.
        /// </summary>
        /// <param name="thisObject">
        /// Input Image stream.
        /// </param>
        /// <param name="width">
        /// The width, in pixels, of the requested thumbnail image.
        /// </param>
        /// <param name="height">
        /// The height, in pixels, of the requested thumbnail image.
        /// </param>
        /// <param name="format">
        /// Format of the thumbnail.
        /// </param>
        /// <returns>
        /// Thumbnail image.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The bitmap object is being returned to caller. Cannot dispose here.")]
        public static Stream GenerateThumbnail(this Stream thisObject, int width, int height, ImageFormat format)
        {
            Stream thumbnailStream = null;

            if (thisObject != null)
            {
                try
                {
                    Bitmap input = new Bitmap(thisObject);
                    thumbnailStream = input.GenerateThumbnail(width, height, format);
                }
                catch
                {
                    // Ignore all exceptions.
                }
            }

            return thumbnailStream;
        }
    }
}