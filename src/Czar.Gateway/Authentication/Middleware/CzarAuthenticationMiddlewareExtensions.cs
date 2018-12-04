using Ocelot.Middleware.Pipeline;

namespace Czar.Gateway.Authentication.Middleware
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-15
    /// 使用自定义授权中间件
    /// </summary>
    public static class CzarAuthenticationMiddlewareExtensions
    {
        public static IOcelotPipelineBuilder UseAhphAuthenticationMiddleware(this IOcelotPipelineBuilder builder)
        {
            return builder.UseMiddleware<CzarAuthenticationMiddleware>();
        }
    }
}
