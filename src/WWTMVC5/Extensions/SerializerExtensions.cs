//-----------------------------------------------------------------------
// <copyright file="SerializerExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Serialization
    /// </summary>
    public static class SerializerExtensions
    {
        /// <summary>
        /// Serializes the XML content and returns the content as a memory stream.
        /// </summary>
        /// <returns>Stream having the xml string.</returns>
        public static Stream GetXmlStream<T>(this T thisObject)
        {
            MemoryStream stream = null;
            try
            {
                var sb = new StringBuilder();
                var x = new XmlSerializer(typeof(T));
                using (var xw = XmlWriter.Create(sb))
                {
                    var emptyNamespace = new XmlSerializerNamespaces();
                    emptyNamespace.Add(string.Empty, string.Empty);
                    x.Serialize(xw, thisObject, emptyNamespace);
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sb.ToString());
                stream = new MemoryStream();
                xmlDoc.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (XmlException)
            {
                // Return null stream in case of exception.
            }

            return stream;
        }

        /// <summary>
        /// Gets the decoded version of the string.
        /// </summary>
        /// <param name="thisObject">Current string.</param>
        /// <returns>Decoded version of the string.</returns>
        public static T DeserializeXML<T>(this string thisObject)
        {
            T result = default(T);
            try
            {
                var x = new XmlSerializer(typeof(T));
                using (var xw = new StringReader(thisObject))
                {
                    result = (T)x.Deserialize(xw);
                }
            }
            catch (XmlException)
            {
                // Return null stream in case of exception.
            }

            return result;
        }
    }
}