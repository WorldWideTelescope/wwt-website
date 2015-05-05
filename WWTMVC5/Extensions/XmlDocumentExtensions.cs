//-----------------------------------------------------------------------
// <copyright file="XmlDocumentExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Content details.
    /// </summary>
    public static class XmlDocumentExtensions
    {
        /// <summary>
        /// Gets the value for the given attribute in the given element of the current xml document.
        /// </summary>
        /// <param name="value">Xml Document object</param>
        /// <param name="elementXPath">XPath of the element.</param>
        /// <param name="attributeName">Attribute whose value to be returned.</param>
        /// <returns>Value of the attribute</returns>
        public static string GetAttributeValue(this XmlDocument value, string elementXPath, string attributeName)
        {
            string attributeValue = string.Empty;

            if (value != null)
            {
                XmlNode firstElement = value.SelectSingleNode(elementXPath);
                if (firstElement != null && firstElement.Attributes[attributeName] != null)
                {
                    attributeValue = firstElement.Attributes[attributeName].Value;
                }
            }

            return attributeValue;
        }

        /// <summary>
        /// Sets the Tour Xml which is parsed from given input stream in the Xml Dom object.
        /// </summary>
        /// <param name="thisObject">Xml Dom object</param>
        /// <param name="fileStream">Stream having the tour file</param>
        /// <returns>Xml set with the tour Xml.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.WebSecurity", "CA3009:Untrusted input should be encoded to avoid potential Xml Injection vulnerabilities", Justification = "Input from Tour file which is Xml file and has a checksum implementation for data integrity.")]
        public static XmlDocument SetXmlFromTour(this XmlDocument thisObject, Stream fileStream)
        {
            try
            {
                ////  Format of the tour file (.wtt) header. 
                ////  Initial section of the tour file will contain the below XML. After that, it will have tour xml content followed by
                ////  binary stream having the tour file contents. Header XML will have size of each file and their offset address in the stream.

                ////  <?xml version='1.0' encoding='UTF-8'?>
                ////  <FileCabinet HeaderSize="0x000003c2">
                ////      <Files>
                ////          <File Name="TC tOUR.wwtxml" Size="9579" Offset="0" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\86858e99-e41a-428e-bf07-5fb7f488488e.thumb.png" Size="1587" Offset="9579" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\e743a6db-1f50-4812-95e1-c7f73fa17010.thumb.png" Size="1375" Offset="11166" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\ed6474d0-7244-42ca-9f7c-4031c085a03f.thumb.png" Size="1617" Offset="12541" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\9747fe35-d9eb-41b0-86b7-82856d2a1488.thumb.png" Size="8778" Offset="14158" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\aa2bca68-b1f9-46a4-92f2-11d572f6a1ab.thumb.png" Size="3166" Offset="22936" />
                ////          <File Name="4d70857b-722d-4739-b9f7-e2fa72df2fc0\4902a150-35fc-45e6-bcee-1aae81a5df7b.png" Size="777835" Offset="26102" />
                ////      </Files>
                ////  </FileCabinet>

                if (fileStream != null)
                {
                    // Reading the HeaderSize attribute's value from the above header xml.
                    fileStream.Seek(0, SeekOrigin.Begin);
                    byte[] buffer = new byte[256];
                    fileStream.Read(buffer, 0, 255);
                    string data = Encoding.UTF8.GetString(buffer);
                    int start = data.IndexOf("0x", StringComparison.Ordinal);

                    // If file is corrupted or invalid format, ignore this tour file.
                    if (start != -1)
                    {
                        // Get the header xml size.
                        int headerSize = Convert.ToInt32(data.Substring(start, 10), 16);
                        fileStream.Seek(0, SeekOrigin.Begin);

                        // Read the header XML from the tour file (.wtt). Using the header size read the header xml stream and load XML dom.
                        buffer = new byte[headerSize];
                        fileStream.Read(buffer, 0, headerSize);
                        data = Encoding.UTF8.GetString(buffer);
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(data);

                        // Get the FileCabinet element.
                        XmlNode cab = doc["FileCabinet"];

                        if (cab != null)
                        {
                            // Get all the File elements.
                            XmlNode files = cab["Files"];

                            if (files != null && files.ChildNodes.Count > 0)
                            {
                                // First File element is for tour xml, get the size of the tour xml.
                                int fileSize = Convert.ToInt32(files.ChildNodes[0].Attributes["Size"].Value, CultureInfo.CurrentCulture);

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    // Read the tour xml stream and load the Tour XML in XML dom.
                                    buffer = new byte[fileSize];
                                    fileStream.Seek(headerSize, SeekOrigin.Begin);
                                    if (fileStream.Read(buffer, 0, fileSize) == fileSize)
                                    {
                                        stream.Write(buffer, 0, fileSize);
                                        stream.Seek(0, SeekOrigin.Begin);
                                        thisObject = new XmlDocument();
                                        thisObject.Load(stream);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (XmlException)
            {
                // Consume any Xml Exception.
                thisObject = null;
            }
            catch (IOException)
            {
                // Consume any IO Exception.
                thisObject = null;
            }

            return thisObject;
        }

        /// <summary>
        /// Sets the WTML Xml which is parsed from given input stream in the Xml Dom object.
        /// </summary>
        /// <param name="thisObject">Xml Dom object</param>
        /// <param name="fileStream">Stream having the WTML file</param>
        /// <returns>Xml set with the WTML Xml.</returns>
        public static XmlDocument SetXmlFromWtml(this XmlDocument thisObject, Stream fileStream)
        {
            try
            {
                ////  Format of the tour file (.wtml) header. 
                ////  WTML file will have only one root node with folder as the element name.

                //// <?xml version='1.0' encoding='UTF-8'?>
                //// <Folder Name='Name of the collection'>
                ////   ...
                //// </Folder>

                if (fileStream != null)
                {
                    thisObject = new XmlDocument();
                    thisObject.Load(fileStream);
                }
            }
            catch (XmlException)
            {
                // Consume any Xml Exception.
                thisObject = null;
            }
            catch (IOException)
            {
                // Consume any IO Exception.
                thisObject = null;
            }

            return thisObject;
        }
    }
}