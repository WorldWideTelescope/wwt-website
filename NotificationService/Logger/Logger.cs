// -
// <copyright file="Logger.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.Research.EarthOnline.NotificationService.Logger
{
    public static class Logger
    {
        /// <summary>
        /// Layerscape TraceSource instance
        /// </summary>
        private static readonly TraceSource defaultTraceSource = new TraceSource("LayerscapeTraceSource");

        /// <summary>
        /// Gets trace source which will be used for tracing and logging
        /// </summary>
        public static TraceSource LayerscapeTraceSource
        {
            get { return defaultTraceSource; }
        }

        /// <summary>
        /// Logs a verbose level message.
        /// </summary>
        /// <param name="id">Trace event id.</param>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Verbose(TraceEventId id, string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Verbose, (int)id, logMessage);
        }

        /// <summary>
        /// Logs a verbose level message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Verbose(string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Verbose, (int)TraceEventId.General, logMessage);
        }

        /// <summary>
        /// Logs a information level message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Info(string format, params object[] args)
        {
            LayerscapeTraceSource.TraceInformation(format, args);
        }

        /// <summary>
        /// Logs a warning level message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Warn(string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Warning, (int)TraceEventId.General, logMessage);
        }

        /// <summary>
        /// Logs a warning level message and exception.
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Warn(Exception exception, string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args, exception);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Warning, (int)TraceEventId.Exception, logMessage);
        }

        /// <summary>
        /// Logs a error level message.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Error(string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Error, (int)TraceEventId.General, logMessage);
        }

        /// <summary>
        /// Logs a error level message and exception.
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Error(Exception exception, string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args, exception);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Error, (int)TraceEventId.Exception, logMessage);
        }

        /// <summary>
        /// Logs a critical level message and exception.
        /// </summary>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Critical(string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Critical, (int)TraceEventId.General, logMessage);
        }

        /// <summary>
        /// Logs a critical level message and exception.
        /// </summary>
        /// <param name="exception">An exception.</param>
        /// <param name="format">The message format.</param>
        /// <param name="args">The message args for formatting.</param>
        public static void Critical(Exception exception, string format, params object[] args)
        {
            string logMessage = FormatMessage(format, args, exception);
            LayerscapeTraceSource.TraceEvent(TraceEventType.Critical, (int)TraceEventId.Exception, logMessage);
        }

        /// <summary>
        /// Method formats the log message.
        /// </summary>
        /// <param name="format">Specifies format message.</param>
        /// <param name="args">Specifies format arguments.</param>
        /// <param name="exception">Specifies exception.</param>
        /// <returns>Returns formatted message.</returns>
        private static string FormatMessage(string format, object[] args, Exception exception = null)
        {
            StringBuilder sb = new StringBuilder();

            // This method is guarded by try...catch block to avoid disasters to the service
            // in case of any issue during formatting.
            try
            {
                sb.AppendFormat(
                    "{0} : {1}", 
                    DateTime.UtcNow,
                    (args != null && args.Length > 0) ? string.Format(format, args) : format);

                if (exception != null)
                {
                    sb.AppendFormat("\nException: {0}", exception.ToString());
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            return sb.ToString();
        }
    }
}
