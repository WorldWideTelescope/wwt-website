//-----------------------------------------------------------------------
// <copyright file="SessionWrapper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.SessionState;
using Microsoft.Live;

namespace WWTMVC5
{
    /// <summary>
    /// Wrapper class around session to handle the scenario when the session is not available.
    /// </summary>
    public class SessionWrapper
    {
        /// <summary>
        /// Initializes a new instance of the SessionWrapper class.
        /// </summary>
        internal SessionWrapper()
        {
        }

        /// <summary>
        /// Gets current session
        /// </summary>
        private static HttpSessionState CurrentSession
        {
            get
            {
                return HttpContext.Current.Session;
            }
        }

        /// <summary>
        /// Gets the object value from the session
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="key">session key</param>
        /// <returns>session object</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All possible errors from session state provider")]
        public static T Get<T>(string key)
        {
            try
            {
                return CurrentSession[key] != null ? (T)CurrentSession[key] : default(T);
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gets the object value from the session
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="key">session key</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>session object or default value</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All possible errors from session state provider")]
        public static T Get<T>(string key, T defaultValue)
        {
            try
            {
                return CurrentSession[key] != null ? (T)CurrentSession[key] : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the object value from the session
        /// </summary>
        /// <typeparam name="T">object type</typeparam>
        /// <param name="key">session key</param>
        /// <param name="value">session object value</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All possible errors from session state provider")]
        public static void Set<T>(string key, T value)
        {
            try
            {
                if (CurrentSession != null)
                {
                    CurrentSession[key] = value;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clears all keys and values from the session-state collection.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "All possible errors from session state provider")]
        public static void Clear()
        {
            try
            {
                if (CurrentSession != null)
                {
                    CurrentSession.Clear();
                }
            }
            catch
            {
            }
        }
    }
    public class LiveConnectClientHandler
    {

        public static LiveConnectClient Client;
        public static LiveLoginResult Result;
    }
}