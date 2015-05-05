//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Web;
using System.Xml;
using WWTMVC5.Models;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for String.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the ContentType of current extension.
        /// </summary>
        /// <param name="thisObject">Current string</param>
        /// <returns>ContentType of the string</returns>
        public static ContentTypes GetContentTypes(this string thisObject)
        {
            ContentTypes type = ContentTypes.Generic;

            if (thisObject != null)
            {
                switch (thisObject.ToUpperInvariant())
                {
                    case ".CSV":
                    case ".XLS":
                    case ".XLSX":
                        type = ContentTypes.Excel;
                        break;
                    case ".DOC":
                    case ".DOCX":
                        type = ContentTypes.Doc;
                        break;
                    case ".WTT":
                        type = ContentTypes.Tours;
                        break;
                    case ".WTML":
                        type = ContentTypes.Wtml;
                        break;
                    case ".WWTL":
                        type = ContentTypes.Wwtl;
                        break;
                    case ".PPT":
                    case ".PPTX":
                        type = ContentTypes.Ppt;
                        break;
                    case ".WMV":
                    case ".MP4":
                        type = ContentTypes.Video;
                        break;
                    default:
                        type = ContentTypes.Generic;
                        break;
                }
            }

            return type;
        }

        /// <summary>
        /// Gets the decoded version of the string.
        /// </summary>
        /// <param name="thisObject">Current string.</param>
        /// <returns>Decoded version of the string.</returns>
        public static string DecodeAndReplace(this string thisObject)
        {
            if (thisObject != null)
            {
                return HttpContext.Current.Server.UrlDecode(thisObject).Replace("<", "&lt;").Replace("\n", "<br />");
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the WWT related Mime Type of current file extension. If the Current extension is not of WWT type. It returns the DefaultMimeType.
        /// </summary>
        /// <param name="thisObject">Current string</param>
        /// <param name="defaultMimeType">Default mime type to be used.</param>
        /// <returns>Mime type of the extension</returns>
        public static string GetWwtMimeType(this string thisObject, string defaultMimeType)
        {
            string mimeType = defaultMimeType;

            if (thisObject != null)
            {
                switch (thisObject.ToUpperInvariant())
                {
                    case ".WTT":
                        mimeType = Constants.WttMimeType;
                        break;
                    case ".WTML":
                        mimeType = Constants.WtmlMimeType;
                        break;
                    case ".WWTL":
                        mimeType = Constants.WwtlMimeType;
                        break;
                    case ".WWTFIG":
                        mimeType = Constants.WwtfigMimeType;
                        break;
                }
            }

            return mimeType;
        }

        /// <summary>
        /// This extension method can be used to fix the email Address of the following incorrect format to correct format.
        ///     Incorrect format :- wwttestuser2%live.com@passport.com, wwtclienttest1%live.com@passport.com
        ///     Correct format :- wwttestuser2@live.com, wwtclienttest1@live.com
        /// </summary>
        /// <param name="thisObject">Current email address in string.</param>
        /// <returns>Corrected email address.</returns>
        public static string FixEmailAddress(this string thisObject)
        {
            string corrected = thisObject;
            if (!string.IsNullOrWhiteSpace(thisObject) && thisObject.Contains("%") && thisObject.Contains("@passport.com"))
            {
                corrected = thisObject.Substring(0, thisObject.IndexOf("@passport.com", StringComparison.OrdinalIgnoreCase)).Replace("%", "@");
            }

            return corrected;
        }

        /// <summary>
        /// Extracts the text from the given HTML string and removes all the HTML tags which are
        /// added for style. If any HTML string is given as text input by user, which will be retained as text as is.
        /// </summary>
        /// <param name="htmlValue">String containing the HTML</param>
        /// <returns>Extracted text if successful. In case of invalid input, i.e. not valid HTML, same input string will be returned.</returns>
        public static string GetTextFromHtmlString(this string htmlValue)
        {
            string innerText = string.Empty;

            try
            {
                string xmlHtmlValue = string.Format(CultureInfo.CurrentCulture, "<root>{0}</root>", htmlValue);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlHtmlValue);

                foreach (XmlNode element in xmlDoc.SelectNodes("//text()"))
                {
                    innerText = string.Format(CultureInfo.CurrentCulture, "{0}{1}", innerText, element.Value);
                }

                return innerText.Trim();
            }
            catch (XmlException)
            {
                // Consume any XmlException and return the input as is.
                return htmlValue;
            }
        }
    }
}