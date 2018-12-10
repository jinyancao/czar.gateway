using Ocelot.Middleware.Pipeline;

namespace Czar.Gateway.Responder.Middleware
{
    /// <summary>
    /// 金焰的世界
    /// 2018-12-10
    /// 应用输出中间件扩展
    /// </summary>
    public static class CzarResponderMiddlewareExtensions
    {
        public static IOcelotPipelineBuilder UseCzarResponderMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<CzarResponderMiddleware>();
        }
    }
}
