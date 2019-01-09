using Ocelot.Middleware.Pipeline;

namespace Czar.Gateway.Rpc.Middleware
{
    public static class CzarRpcMiddlewareExtensions
    {
        public static IOcelotPipelineBuilder UseCzarRpcMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<CzarRpcMiddleware>();
        }
    }
}
