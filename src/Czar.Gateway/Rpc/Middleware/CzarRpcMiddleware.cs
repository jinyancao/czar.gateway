using Czar.Gateway.Errors;
using Czar.Rpc.Clients;
using Ocelot.Logging;
using Ocelot.Middleware;
using Ocelot.Responses;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Czar.Gateway.Rpc.Middleware
{
    public class CzarRpcMiddleware : OcelotMiddleware
    {
        private readonly OcelotRequestDelegate _next;
        private readonly IRpcClientFactory _clientFactory;
        private readonly ICzarRpcProcessor _czarRpcProcessor;
        public CzarRpcMiddleware(OcelotRequestDelegate next, IRpcClientFactory clientFactory,
            IOcelotLoggerFactory loggerFactory, ICzarRpcProcessor czarRpcProcessor) : base(loggerFactory.CreateLogger<CzarRpcMiddleware>())
        {
            _next = next;
            _clientFactory = clientFactory;
            _czarRpcProcessor = czarRpcProcessor;
        }

        public async Task Invoke(DownstreamContext context)
        {
            var httpStatusCode = HttpStatusCode.OK;
            var _param = new List<object>();
            //1、提取路由参数
            var tmpInfo = context.TemplatePlaceholderNameAndValues;
            if (tmpInfo != null && tmpInfo.Count > 0)
            {
                foreach (var tmp in tmpInfo)
                {
                    _param.Add(tmp.Value);
                }
            }
            //2、提取query参数
            foreach (var _q in context.HttpContext.Request.Query)
            {
                _param.Add(_q.Value.ToString());
            }
            //3、从body里提取内容
            if (context.HttpContext.Request.Method.ToUpper() != "GET")
            {
                context.DownstreamRequest.Scheme = "http";
                var requert = context.DownstreamRequest.ToHttpRequestMessage();
                if (requert.Content!=null)
                {
                    var json = "{}";
                    json = await requert.Content.ReadAsStringAsync();
                    _param.Add(json);
                }
            }
            //从缓存里提取
            var req = await _czarRpcProcessor.GetRemoteMethodAsync(context.DownstreamReRoute.UpstreamPathTemplate.OriginalValue);
            if (req != null)
            {
                req.Parameters = _param.ToArray();
                var result = await _clientFactory.SendAsync(req, GetEndPoint(context.DownstreamRequest.Host, context.DownstreamRequest.Port));
                OkResponse<RpcHttpContent> httpResponse;
                if (result.CzarCode == Czar.Rpc.Utilitys.RpcStatusCode.Success)
                {
                    httpResponse = new OkResponse<RpcHttpContent>(new RpcHttpContent(result.CzarResult?.ToString()));
                }
                else
                {
                    httpResponse = new OkResponse<RpcHttpContent>(new RpcHttpContent(result));
                }
                context.HttpContext.Response.ContentType = "application/json";
                context.DownstreamResponse = new DownstreamResponse(httpResponse.Data, httpStatusCode, httpResponse.Data.Headers, "OK");
            }
            else
            {//输出错误
                var error = new InternalServerError($"请求路由 {context.HttpContext.Request.Path}未配置后端转发");
                Logger.LogWarning($"{error}");
                SetPipelineError(context, error);
            }
        }
        private EndPoint GetEndPoint(string ipaddress, int port)
        {
            if (IPAddress.TryParse(ipaddress, out IPAddress ip))
            {
                return new IPEndPoint(ip, port);
            }
            else
            {
                return new DnsEndPoint(ipaddress, port);
            }
        }
    }
}
