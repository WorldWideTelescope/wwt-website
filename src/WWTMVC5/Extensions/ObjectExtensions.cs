//-----------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Xml.Serialization;
using WWTMVC5.Properties;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for all Objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Checks whether the given parameters are null or not. In case of null, throws Argument null exception.
        /// </summary>
        /// <typeparam name="T">Type of object on which check method is called</typeparam>
        /// <param name="o">Object on which check method is called</param>
        /// <param name="parameters">List of parameters (generally objects) to be checked for null</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "We are validating this parameter using reflection."),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "o", Justification = "Need to have check null for all objects.")]
        public static void CheckNotNull<T>(this T o, Func<object> parameters)
        {
            var objects = parameters();
            var properties = objects.GetType().GetProperties();

            // Find the parameters/objects which are null and get their name.
            var emptyProperties = properties.Where(p => p.GetValue(objects, null) == null).Select(p => p.Name);

            if (emptyProperties.Any())
            {
                throw new HttpException(404, Resources.ItemNotExists);
            }
        }

        /// <summary>
        /// Serializes the object to XML file.
        /// </summary>
        /// <param name="classObject">The class object.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void WriteToXmlFile<TResult>(this TResult classObject, string fileName)
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(TResult));
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlFormat.Serialize(stream, classObject);
            }
        }

        /// <summary>
        /// Serializes the object to binary file.
        /// </summary>
        /// <param name="classObject">The class object.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void WriteToBinaryFile<TResult>(this TResult classObject, string fileName)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binaryFormatter.Serialize(stream, classObject);
            }
        }
    }
}