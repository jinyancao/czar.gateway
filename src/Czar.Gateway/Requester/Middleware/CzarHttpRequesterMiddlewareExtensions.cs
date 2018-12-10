using Ocelot.Middleware.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;

namespace Czar.Gateway.Requester.Middleware
{
    public static class CzarHttpRequesterMiddlewareExtensions
    {
        public static IOcelotPipelineBuilder UseCzaHttpRequesterMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<CzarHttpRequesterMiddleware>();
        }
    }
}
