using Ocelot.Middleware.Pipeline;

namespace Czar.Gateway.RateLimit.Middleware
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-18
    /// 限流中间件扩展
    /// </summary>
    public static class CzarClientRateLimitMiddlewareExtensions
    {
        public static IOcelotPipelineBuilder UseCzarClientRateLimitMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<CzarClientRateLimitMiddleware>();
        }
    }
}
