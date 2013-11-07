//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;


[assembly: CLSCompliant(false)]
namespace ActorMovieTwelve
{
    public class ActorMovieDataService : DataService<martinbeEntities>
    {
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");

            }
            
            config.SetEntitySetAccessRule("Actor", EntitySetRights.All);
            config.SetEntitySetAccessRule("Movie", EntitySetRights.All);
            config.MaxExpandDepth = 1;
            config.UseVerboseErrors = true;
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
        }
    }
}
