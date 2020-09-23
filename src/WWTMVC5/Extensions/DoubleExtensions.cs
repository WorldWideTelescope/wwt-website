//-----------------------------------------------------------------------
// <copyright file="DoubleExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;

namespace WWTMVC5.Extensions
{
    /// <summary>
    /// Class having the extension methods needed for Double.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Returns string with binary notation of value rounded to 2 decimal places.
        /// For example 
        ///     123="123 B"
        ///     2345="2.29 KB"
        ///     1234567="1.18 MB"
        /// </summary>
        /// <param name="thisObject">Numeric to convert.</param>
        /// <returns>The string representation of the decimal/Double</returns>
        public static string FormatBytes(this double thisObject)
        {
            double value = 0;
            int suffixIndex = 0;
            string[] suffix = { "B", "KB", "MB", "GB", "TB", "PB" };
            for (int index = suffix.GetUpperBound(0); index > 0; index--)
            {
                double baseValue = Math.Pow(1024, index);
                if (thisObject >= baseValue)
                {
                    value = thisObject / baseValue;
                    suffixIndex = index;
                    break;
                }
            }

            return string.Format(CultureInfo.InvariantCulture, "{0:0.00} {1}", value, suffix[suffixIndex]);
        }
    }
}