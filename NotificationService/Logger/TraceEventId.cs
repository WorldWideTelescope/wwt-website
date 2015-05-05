//-----------------------------------------------------------------------
// <copyright file="TraceEventId.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Research.EarthOnline.NotificationService.Logger
{
    /// <summary>
    /// Enum specifies the trace event ids.
    /// </summary>
    public enum TraceEventId
    {
        /// <summary>
        /// Specifies a General id.
        /// </summary>
        General = 0,

        /// <summary>
        /// Specifies a method entry.
        /// </summary>
        Entry = 1,

        /// <summary>
        /// Specifies a method exit.
        /// </summary>
        Exit = 2,

        /// <summary>
        /// Specifies an exception.
        /// </summary>
        Exception = 100,

        /// <summary>
        /// Specifies an unexpected error.
        /// </summary>
        Unexpected = 200,

        /// <summary>
        /// Specifies a program flow.
        /// </summary>
        Flow = 300
    }
}