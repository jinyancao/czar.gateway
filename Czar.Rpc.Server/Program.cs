using Czar.Rpc.Codec;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Czar.Rpc.Extensions;
using Czar.Rpc.DotNetty.Extensions;
namespace Czar.Rpc.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureHostConfiguration(i => i.AddJsonFile("CzarConfig.json"))
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                })
                .UseCodec<JsonCodec>()
                .UseLibuvTcpHost()
                .UseProxy()
                .UseConsoleLifetime()
                .Build();

            host.RunAsync().Wait();
        }
    }
}
