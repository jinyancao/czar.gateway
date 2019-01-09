using System;
using System.Threading.Tasks;
using Czar.Gateway.Authentication.Middleware;
using Czar.Gateway.RateLimit.Middleware;
using Czar.Gateway.Requester.Middleware;
using Czar.Gateway.Responder.Middleware;
using Czar.Gateway.Rpc.Middleware;
using Ocelot.Authentication.Middleware;
using Ocelot.Authorisation.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.Middleware;
using Ocelot.Middleware.Pipeline;
using Ocelot.RateLimit.Middleware;
using Ocelot.Request.Middleware;
using Ocelot.Requester.Middleware;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using Ocelot.WebSockets.Middleware;

namespace Czar.Gateway.Middleware
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-15
    /// 网关管道扩展
    /// </summary>
    public static class OcelotPipelineExtensions
    {
        public static OcelotRequestDelegate BuildCzarOcelotPipeline(this IOcelotPipelineBuilder builder,
            OcelotPipelineConfiguration pipelineConfiguration)
        {
           
            // 注册一个全局异常
            builder.UseExceptionHandlerMiddleware();

            // 如果请求是websocket使用单独的管道
            builder.MapWhen(context => context.HttpContext.WebSockets.IsWebSocketRequest,
                app =>
                {
                    app.UseDownstreamRouteFinderMiddleware();
                    app.UseDownstreamRequestInitialiser();
                    app.UseLoadBalancingMiddleware();
                    app.UseDownstreamUrlCreatorMiddleware();
                    app.UseWebSocketsProxyMiddleware();
                });

            // 添加自定义的错误管道
            builder.UseIfNotNull(pipelineConfiguration.PreErrorResponderMiddleware);

            //使用自定义的输出管道
            builder.UseCzarResponderMiddleware();

            // 下游路由匹配管道
            builder.UseDownstreamRouteFinderMiddleware();

            //增加自定义扩展管道
            if (pipelineConfiguration.MapWhenOcelotPipeline != null)
            {
                foreach (var pipeline in pipelineConfiguration.MapWhenOcelotPipeline)
                {
                    builder.MapWhen(pipeline);
                }
            }

            // 使用Http头部转换管道
            builder.UseHttpHeadersTransformationMiddleware();

            // 初始化下游请求管道
            builder.UseDownstreamRequestInitialiser();

            // 使用自定义限流管道
            builder.UseRateLimiting();

            //使用请求ID生成管道
            builder.UseRequestIdMiddleware();

            //使用自定义授权前管道
            builder.UseIfNotNull(pipelineConfiguration.PreAuthenticationMiddleware);

            //根据请求判断是否启用授权来使用管道
            if (pipelineConfiguration.AuthenticationMiddleware == null)
            {
                builder.UseAuthenticationMiddleware();
            }
            else
            {
                builder.Use(pipelineConfiguration.AuthenticationMiddleware);
            }

            //添加自定义限流中间件 2018-11-18 金焰的世界
            builder.UseCzarClientRateLimitMiddleware();

            //添加自定义授权中间件  2018-11-15 金焰的世界
            builder.UseAhphAuthenticationMiddleware();

            //启用自定义的认证之前中间件
            builder.UseIfNotNull(pipelineConfiguration.PreAuthorisationMiddleware);

            //是否使用自定义的认证中间件
            if (pipelineConfiguration.AuthorisationMiddleware == null)
            {
                builder.UseAuthorisationMiddleware();
            }
            else
            {
                builder.Use(pipelineConfiguration.AuthorisationMiddleware);
            }

            // 使用自定义的参数构建中间件
            builder.UseIfNotNull(pipelineConfiguration.PreQueryStringBuilderMiddleware);

            // 使用负载均衡中间件
            builder.UseLoadBalancingMiddleware();

            // 使用下游地址创建中间件
            builder.UseDownstreamUrlCreatorMiddleware();

            // 使用缓存中间件
            builder.UseOutputCacheMiddleware();

            //判断下游的是否启用rpc通信,切换到RPC处理
            builder.MapWhen(context => context.DownstreamReRoute.DownstreamScheme.Equals("rpc", StringComparison.OrdinalIgnoreCase), app =>
            {
                app.UseCzarRpcMiddleware();
            });

            //使用下游请求中间件
            builder.UseCzaHttpRequesterMiddleware();

            return builder.Build();
        }

        private static void UseIfNotNull(this IOcelotPipelineBuilder builder,
            Func<DownstreamContext, Func<Task>, Task> middleware)
        {
            if (middleware != null)
            {
                builder.Use(middleware);
            }
        }
    }
}
