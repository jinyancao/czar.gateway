using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Czar.Gateway.Rpc
{
    public class RpcHttpContent : HttpContent
    {
        private string result;

        public RpcHttpContent(string result)
        {
            this.result = result;
        }

        public RpcHttpContent(object result)
        {
            this.result = Newtonsoft.Json.JsonConvert.SerializeObject(result);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var writer = new StreamWriter(stream);
            await writer.WriteAsync(result);
            await writer.FlushAsync();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = result.Length;
            return true;
        }
    }
}
