using AspectCore.Extensions.DependencyInjection;
using Czar.Rpc.Codec;
using Czar.Rpc.Common;
using Czar.Rpc.Diagnostics;
using Czar.Rpc.DotNetty.Extensions;
using Czar.Rpc.Exceptions;
using Czar.Rpc.Extensions;
using Czar.Rpc.Message;
using DotNetty.Common.Internal.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Czar.Rpc.Client
{
    class Program
    {
        public static IServiceProvider service;
        public static IConfiguration config;
        static async Task Main(string[] args)
        {
            List<string> list_error = new List<string>();
            try
            {
                var builder = new ConfigurationBuilder();
                config = builder.AddJsonFile("CzarConfig.json").Build();
                
              service = new ServiceCollection()
                    .AddSingleton(config)
                    .AddLogging(j => j.AddConsole())
                    .AddLibuvTcpClient(config)
                    .AddProxy()
                    .BuildDynamicProxyServiceProvider();

                var rpc = service.GetRequiredService<IHelloRpc>();
                rpc.CzarEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7711);
                var result = string.Empty;
                
                string t = "基本调用";
                result = rpc.Hello(18, t);
                Console.WriteLine(result);

                result = "无返回结果";
                rpc.HelloHolder(1, out result);
                Console.WriteLine(result);
                result = await rpc.HelloTask(2, "异步任务");
                Console.WriteLine(result);
                result = "单向";
                await rpc.HelloOneway(3, "单向调用");
                Console.WriteLine(result);
                result = await rpc.HelloValueTask(4, "ValueTask任务");
                Console.WriteLine(result);

                var modelResult = rpc.HelloModel(5, "返回实体", DateTime.Now);
                Console.WriteLine($"{modelResult.T1} {modelResult.T2} {modelResult.T3.ToLongDateString()}");


                var modelResult1 = await rpc.HelloModelAsync(6, "返回Task实体", DateTime.Now);
                Console.WriteLine($"{modelResult1.T1} {modelResult1.T2} {modelResult1.T3.ToLongDateString()}");

                var mm = new DemoModel()
                {
                    T1 = 7,
                    T2 = "传实体返回实体",
                    T3 = DateTime.Now,
                    Child = new ChildModel()
                    {
                        C1 = "子类1"
                    }
                };
                var model2 = rpc.HelloSendModel(mm);
                Console.WriteLine($"{model2.T1} {model2.T2} {model2.T3.ToLongDateString()}  {model2.Child.C1}");

                var list = new List<DemoModel>();
                var mm1 = new DemoModel()
                {
                    T1 = 8,
                    T2 = "传List返回List",
                    T3 = DateTime.Now,
                    Child = new ChildModel()
                    {
                        C1 = "子类2"
                    }
                };
                var mm3 = new DemoModel()
                {
                    T1 = 9,
                    T2 = "传List返回List",
                    T3 = DateTime.Now,
                    Child = new ChildModel()
                    {
                        C1 = "子类3"
                    }
                };
                list.Add(mm1);
                list.Add(mm3);
                var list3 = rpc.HelloSendModelList(list);
                Console.WriteLine($"{list3[0].T1} {list3[0].T2} {list3[0].T3.ToLongDateString()} {list3[0].Child?.C1}");


                var mm4 = new DemoModel()
                {
                    T1 = 9,
                    T2 = "HelloSendModelParm",
                    T3 = DateTime.Now,
                    Child = new ChildModel()
                    {
                        C1 = "子类4"
                    }
                };
                var dd = rpc.HelloSendModelParm("HelloSendModelParm", mm4);
                Console.WriteLine($"{dd.T1} {dd.T2} {dd.T3.ToLongDateString()}  {dd.Child.C1}");

                //异常调用
                await rpc.TestBusinessExceptionInterceptor();
            }
            catch (BusinessException e)
            {
                Console.WriteLine($"CzarCode:{e.CzarCode} CzarMessage:{e.CzarMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();

        }

    }
}
