using Czar.Gateway.Configuration;
using Microsoft.AspNetCore.Http;
using Ocelot.Errors;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Responder;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Czar.Gateway.Responder.Middleware
{
    /// <summary>
    /// 金焰的世界
    /// 2018-12-10
    /// 统一输出中间件
    /// </summary>
    public class CzarResponderMiddleware: OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        private readonly IHttpResponder _responder;
        private readonly IErrorsToHttpStatusCodeMapper _codeMapper;

        public CzarResponderMiddleware(OcelotRequestDelegate next,
            IHttpResponder responder,
            IOcelotLoggerFactory loggerFactory,
            IErrorsToHttpStatusCodeMapper codeMapper
           )
            : base(loggerFactory.CreateLogger<CzarResponderMiddleware>())
        {
            _next = next;
            _responder = responder;
            _codeMapper = codeMapper;
        }

        public async Task Invoke(DownstreamContext context)
        {
            await _next.Invoke(context);

            if (context.IsError)
            {//自定义输出结果
                var errmsg = context.Errors[0].Message;
                int httpstatus = _codeMapper.Map(context.Errors);
                var errResult = new BaseResult() { errcode = httpstatus, errmsg = errmsg };
                var message = errResult.ToJson();
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.HttpContext.Response.WriteAsync(message);
                return;
            }
            else if (context.HttpContext.Response.StatusCode != (int)HttpStatusCode.OK)
            {//标记失败，不做任何处理,自定义扩展时预留

            }
            else if (context.DownstreamResponse == null)
            {//添加如果管道强制终止，不做任何处理,修复未将对象实例化错误

            }
            else
            {//继续请求下游地址返回
                Logger.LogDebug("no pipeline errors, setting and returning completed response");
                await _responder.SetResponseOnHttpContext(context.HttpContext, context.DownstreamResponse);
            }
        }

        private void SetErrorResponse(HttpContext context, List<Error> errors)
        {
            var statusCode = _codeMapper.Map(errors);
            _responder.SetErrorResponseOnContext(context, statusCode);
        }
    }
}
