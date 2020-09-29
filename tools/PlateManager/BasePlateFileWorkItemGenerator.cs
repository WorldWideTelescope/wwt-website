using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlateManager
{
    /// <summary>
    /// Shared logic that the different plate work item generators will reuse
    /// </summary>
    public abstract class BasePlateFileWorkItemGenerator
    {
        /// <summary>
        /// Reads a file into a memory stream
        /// </summary>
        /// <param name="filename">The file to read into a memory stream</param>
        /// <returns>A memory stream derived from the file specified by filename</returns>
        protected Stream GetFileStream(string filename)
        {
            byte[] data = File.ReadAllBytes(filename);
            return new MemoryStream(data);
        }

        /// <summary>
        /// Gets the name of the expected JPG from the plate file path
        /// </summary>
        /// <param name="plateFile">The full name and path of the input plate file</param>
        /// <returns>The full name of a thumbnail that would sit alongside the plate file with the same base file name</returns>
        protected static string GetThumbnailName(string plateFile) => plateFile.Replace(".plate", ".jpg").ToLower().Replace("-", "_");

        /// <summary>
        /// Takes the base name of the plate and returns the name of the thumbnail that should be created in azure blob storage
        /// </summary>
        /// <param name="thumbnailName">The base name of the plate file</param>
        /// <returns>The appended and cased string</returns>
        protected static string GetThumbnailBlobName(string thumbnailName) => thumbnailName.ToLower().Replace("-", "_") + "_thumb.jpg";

        /// <summary>
        /// Gets the name of a WTML file that would sit alongside the plate file
        /// </summary>
        /// <param name="plateFile">The full name and path of the input plate file</param>
        /// <returns>The full name of the WTML file that would sit alongside the plate file with the same base file name</returns>
        protected static string GetWtmlName(string plateFile) => plateFile.Replace(".plate", ".wtml").ToLower();

        /// <summary>
        /// Replaces entries in the WTML file with new entries that point to the blob storage endpoint
        /// </summary>
        /// <param name="wtmlData">The wtml file, ingested into a string</param>
        /// <param name="plateName">The base name of the plate, without a file extension</param>
        /// <param name="baseUrl">The URL of the storage endpoint</param>
        /// <param name="azureContainer">The blob container name</param>
        /// <returns></returns>
        protected static string UpdateWtmlEntries(string wtmlData, string plateName, string baseUrl, string azureContainer) =>
            wtmlData.Replace(plateName + "/{1}/{3}/{3}_{2}.png", baseUrl + azureContainer + "/" + plateName + "L{1}X{2}Y{3}.png")
                    .Replace(plateName.ToLower().Replace("-", "_") + ".jpg", baseUrl + azureContainer + "/" + plateName.ToLower().Replace("-", "_") + "_thumb.jpg");
    }
}
