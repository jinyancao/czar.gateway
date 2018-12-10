using Czar.Gateway.Errors;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Requester;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czar.Gateway.Requester.Middleware
{
    public class CzarHttpRequesterMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        private readonly IHttpRequester _requester;

        public CzarHttpRequesterMiddleware(OcelotRequestDelegate next,
            IOcelotLoggerFactory loggerFactory,
            IHttpRequester requester)
                : base(loggerFactory.CreateLogger<CzarHttpRequesterMiddleware>())
        {
            _next = next;
            _requester = requester;
        }

        public async Task Invoke(DownstreamContext context)
        {
            var response = await _requester.GetResponse(context);

            if (response.IsError)
            {
                Logger.LogDebug("IHttpRequester returned an error, setting pipeline error");

                SetPipelineError(context, response.Errors);
                return;
            }
            else if(response.Data.StatusCode != System.Net.HttpStatusCode.OK)
            {//如果后端未处理异常，设置异常信息，统一输出，防止暴露敏感信息
                var error = new InternalServerError($"请求服务异常");
                Logger.LogWarning($"路由地址 {context.HttpContext.Request.Path} 请求服务异常. {error}");
                SetPipelineError(context, error);
                return;
            }
            Logger.LogDebug("setting http response message");

            context.DownstreamResponse = new DownstreamResponse(response.Data);
        }
    }
}
