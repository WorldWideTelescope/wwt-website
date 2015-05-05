//-----------------------------------------------------------------------
// <copyright file="OperationStatus.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace WWTMVC5.Models
{
    /// <summary>
    /// Class representing the status of the operation performed by the service method.
    /// </summary>
    [Serializable]
    public class OperationStatus
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operations is succeeded or not.
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a custom error message is sent from service or not.
        /// </summary>
        public bool CustomErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the custom error message.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the exception, if any occurred.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Create a success Operation status message.
        /// </summary>
        /// <returns>Successful Operation status</returns>
        public static OperationStatus CreateSuccessStatus()
        {
            return new OperationStatus()
            {
                Succeeded = true
            };
        }

        /// <summary>
        /// Create a failure Operation status message.
        /// </summary>
        /// <param name="exception">Exception occurred.</param>
        /// <returns>Failure Operation status.</returns>
        public static OperationStatus CreateFailureStatus(Exception exception)
        {
            return new OperationStatus()
            {
                Succeeded = false,
                Exception = exception,
                CustomErrorMessage = false,
                ErrorMessage = exception != null ? exception.Message : string.Empty
            };
        }

        /// <summary>
        /// Create a failure Operation status message.
        /// </summary>
        /// <param name="errorMessage">Custom error message.</param>
        /// <returns>Failure Operation status.</returns>
        public static OperationStatus CreateFailureStatus(string errorMessage)
        {
            return new OperationStatus()
            {
                Succeeded = false,
                CustomErrorMessage = true,
                ErrorMessage = errorMessage
            };
        }

        /// <summary>
        /// Create a failure Operation status message.
        /// </summary>
        /// <param name="errorMessage">Custom error message.</param>
        /// <param name="exception">Exception occurred.</param>
        /// <returns>Failure Operation status.</returns>
        public static OperationStatus CreateFailureStatus(string errorMessage, Exception exception)
        {
            return new OperationStatus()
            {
                Succeeded = false,
                Exception = exception,
                CustomErrorMessage = true,
                ErrorMessage = errorMessage
            };
        }
    }
}