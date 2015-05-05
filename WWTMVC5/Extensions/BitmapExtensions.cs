//-----------------------------------------------------------------------
// <copyright file="BitmapExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Bitmap.
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        /// This function is used to Generate the thumbnail of the input image.
        /// </summary>
        /// <param name="thisObject">
        /// Input Image.
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
        public static Stream GenerateThumbnail(this Bitmap thisObject, int width, int height, ImageFormat format)
        {
            MemoryStream thumbnailStream = null;
            if (thisObject != null)
            {
                try
                {
                    Bitmap thumbnail = thisObject.GenerateThumbnail(width, height);

                    if (thumbnail != null)
                    {
                        thumbnailStream = new MemoryStream();
                        thumbnail.Save(thumbnailStream, format);
                    }
                }
                catch
                {
                    // Ignore all exceptions.
                }
            }

            return thumbnailStream;
        }

        /// <summary>
        /// This function is used to create the thumbnail of the input image.
        /// </summary>
        /// <param name="input">
        /// Input Image.
        /// </param>
        /// <param name="width">
        /// The width, in pixels, of the requested thumbnail image.
        /// </param>
        /// <param name="height">
        /// The height, in pixels, of the requested thumbnail image.
        /// </param>
        /// <returns>
        /// Thumbnail image.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Need to ignore any exception which occurs.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The bitmap object is being returned to caller. Cannot dispose here.")]
        private static Bitmap GenerateThumbnail(this Bitmap input, int width, int height)
        {
            Bitmap image = null;
            if (input != null)
            {
                image = new Bitmap(width, height);

                Graphics graphics = Graphics.FromImage(image);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.Clear(Color.Black);

                int originalWidth = input.Width;
                int originalHeight = input.Height;

                int revisedWidth = originalWidth;
                int revisedHeight = originalHeight;

                double aspectRatioThumbnail = ((double)width) / ((double)height);
                double aspectRatioInput = ((double)originalWidth) / ((double)originalHeight);

                if (aspectRatioInput < aspectRatioThumbnail)
                {
                    revisedWidth = (int)(originalHeight * aspectRatioThumbnail);
                }
                else
                {
                    revisedHeight = (int)(originalWidth / aspectRatioThumbnail);
                }

                int offsetY = 0;
                if (originalHeight != revisedHeight)
                {
                    offsetY = (originalHeight - revisedHeight) / 2;
                }

                int offsetX = 0;
                if (originalWidth != revisedWidth)
                {
                    offsetX = (originalWidth - revisedWidth) / 2;
                }

                Rectangle destinationRect = new Rectangle(0, 0, width, height);
                Rectangle sourceRect = new Rectangle(offsetX, offsetY, revisedWidth, revisedHeight);

                try
                {
                    graphics.DrawImage(input, destinationRect, sourceRect, GraphicsUnit.Pixel);
                }
                catch
                {
                    // Ignore any exception.
                }

                graphics.Dispose();
            }

            return image;
        }
    }
}