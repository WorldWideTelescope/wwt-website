//-----------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using WWTMVC5.Properties;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Returns string representation of the Difference between the date times.
        /// </summary>
        /// <returns>The string representation of the DateTime.</returns>
        public static string GetFormattedDifference(this DateTime datetime1, DateTime datetime2)
        {
            string value = string.Empty;
            TimeSpan difference = datetime2.Subtract(datetime1);
            if (difference.Days > 0)
            {
                value = string.Format(CultureInfo.CurrentCulture, Resources.DaysAgoText, difference.Days, difference.Days == 1 ? string.Empty : "s");
            }
            else if (difference.Hours > 0)
            {
                value = string.Format(CultureInfo.CurrentCulture, Resources.HoursAgoText, difference.Hours, difference.Hours == 1 ? string.Empty : "s");
            }
            else if (difference.Minutes > 0)
            {
                value = string.Format(CultureInfo.CurrentCulture, Resources.MinutesAgoText, difference.Minutes, difference.Minutes == 1 ? string.Empty : "s");
            }
            else
            {
                value = string.Format(CultureInfo.CurrentCulture, Resources.SecondsAgoText, difference.Seconds > 0 ? difference.Seconds : 1, difference.Seconds == 1 ? string.Empty : "s");
            }

            return value;
        }
    }
}