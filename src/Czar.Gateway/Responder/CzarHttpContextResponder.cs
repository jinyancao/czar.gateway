using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Ocelot.Headers;
using Ocelot.Middleware;
using Ocelot.Responder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Czar.Gateway.Responder
{
    public class CzarHttpContextResponder : IHttpResponder
    {
        private readonly IRemoveOutputHeaders _removeOutputHeaders;

        public CzarHttpContextResponder(IRemoveOutputHeaders removeOutputHeaders)
        {
            _removeOutputHeaders = removeOutputHeaders;
        }

        public async Task SetResponseOnHttpContext(HttpContext context, DownstreamResponse response)
        {
            _removeOutputHeaders.Remove(response.Headers);

            foreach (var httpResponseHeader in response.Headers)
            {
                AddHeaderIfDoesntExist(context, httpResponseHeader);
            }

            foreach (var httpResponseHeader in response.Content.Headers)
            {
                AddHeaderIfDoesntExist(context, new Header(httpResponseHeader.Key, httpResponseHeader.Value));
            }

            var content = await response.Content.ReadAsStreamAsync();

            if (response.Content.Headers.ContentLength != null)
            {
                AddHeaderIfDoesntExist(context, new Header("Content-Length", new[] { content.Length.ToString() }));
            }

            SetStatusCode(context, (int)response.StatusCode);

            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = response.ReasonPhrase;

            using (content)
            {
                if (response.StatusCode != HttpStatusCode.NotModified && context.Response.ContentLength != 0)
                {
                    await content.CopyToAsync(context.Response.Body);
                }
            }
        }

        public void SetErrorResponseOnContext(HttpContext context, int statusCode)
        {
            SetStatusCode(context, statusCode);
        }

        private void SetStatusCode(HttpContext context, int statusCode)
        {
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = statusCode;
            }
        }

        private static void AddHeaderIfDoesntExist(HttpContext context, Header httpResponseHeader)
        {
            if (!context.Response.Headers.ContainsKey(httpResponseHeader.Key))
            {
                context.Response.Headers.Add(httpResponseHeader.Key, new StringValues(httpResponseHeader.Values.ToArray()));
            }
        }
    }
}
