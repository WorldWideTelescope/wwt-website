//-----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation 2011. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.ServiceModel.Web;
using Microsoft.Live;
using WWTMVC5.Models;

namespace WWTMVC5.WebServices
{
    [SuppressMessage("Microsoft.Design", "CA1053:StaticHolderTypesShouldNotHaveConstructors", Justification = "Can't create a private constructor.")]
    public class ServiceBase
    {
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "We are creating a new instance of profileDetails every time.")]
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "It is required as per the design.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "RPS calls")]
        

        protected static long ValidateEntityId(string id)
        {
            long result = 0;
            if (!long.TryParse(id, out result))
            {
                throw new WebFaultException<string>("Invalid ID", HttpStatusCode.BadRequest);
            }

            return result;
        }

        protected static Guid ValidateGuid(string guid)
        {
            Guid result = Guid.Empty;
            if (!Guid.TryParse(guid, out result))
            {
                throw new WebFaultException<string>("Invalid GUID", HttpStatusCode.BadRequest);
            }

            return result;
        }
    }
}