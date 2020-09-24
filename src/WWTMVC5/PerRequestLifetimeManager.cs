//-----------------------------------------------------------------------
// <copyright file="PerRequestLifetimeManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web;
using Microsoft.Practices.Unity;

namespace WWTMVC5
{
    /// <summary>
    /// When you register an object in the unity container, you will need to specify the life time of the object 
    /// unless you want the default lifetime to be assigned
    /// </summary>
    public class PerRequestLifetimeManager : LifetimeManager, IDisposable
    {
        private readonly Guid key = Guid.NewGuid();

        /// <summary>
        /// Gets the value of the key from the store
        /// </summary>
        /// <returns>object associated with key</returns>
        public override object GetValue()
        {
            if (HttpContext.Current != null && HttpContext.Current.Items.Contains(key))
            {
                return HttpContext.Current.Items[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the key from the store
        /// </summary>
        public override void RemoveValue()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items.Remove(key);
            }
        }

        /// <summary>
        /// Stores the value of the key.
        /// </summary>
        /// <param name="newValue">value for the key</param>
        public override void SetValue(object newValue)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[key] = newValue;
            }
        }

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose when true
        /// </summary>
        /// <param name="disposing">is disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.RemoveValue();
            }
        }
    }
}