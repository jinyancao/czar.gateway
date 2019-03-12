using Czar.Rpc.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Czar.Rpc.Common;

namespace Demo.Rpc.Server
{
    public class HelloRpcServer: IHelloRpc
    {
        public EndPoint CzarEndPoint { get; set; }

        public string Hello(int no, string name)
        {
            string result = $"{no}: Hi, {name}";
            Console.WriteLine(result);
            return result + " callback";
        }

        public void HelloHolder(int no, out string name)
        {
            name = no.ToString() + " out";
        }

        public async Task HelloOneway(int no, string name)
        {
            await Task.Delay(10000);
            Console.WriteLine($"From oneway - {no}: Hi, {name}");
        }

        public Task<string> HelloTask(int no, string name)
        {
            return Task.FromResult(Hello(no, name));
        }

        public ValueTask<string> HelloValueTask(int no, string name)
        {
            return new ValueTask<string>(Hello(no, name));
        }

        public Task TestBusinessExceptionInterceptor()
        {
            throw new BusinessException()
            {
                CzarCode = "1",
                CzarMessage = "test"
            };
        }

        public DemoModel HelloModel(int D1, string D2, DateTime D3)
        {
            return new DemoModel()
            {
                T1 = D1 + 1,
                T2 = D2 + "2",
                T3 = D3.AddDays(1)
            };
        }

        public async Task<DemoModel> HelloModelAsync(int D1, string D2, DateTime D3)
        {
            return await Task.FromResult(
               new DemoModel()
               {
                   T1 = D1 + 1,
                   T2 = D2 + "77777",
                   T3 = D3.AddDays(1)
               }
                );
        }

        public DemoModel HelloSendModel(DemoModel model)
        {
            model.T1 = model.T1 + 10;
            model.T2 = model.T2 + "11";
            model.T3 = model.T3.AddDays(12);
            return model;
        }

        public DemoModel HelloSendModelParm(string name, DemoModel model)
        {
            model.T1 = model.T1 + 10;
            model.T2 = model.T2 + "11";
            model.T3 = model.T3.AddDays(12);
            if (model.Child != null)
            {
                model.Child.C1 = name+"说:"+ model.Child.C1;
            }
            return model;
        }

        public List<DemoModel> HelloSendModelList(List<DemoModel> model)
        {
            return model.Select(t => new DemoModel() { T1=t.T1+10,T2=t.T2+"13",T3=t.T3.AddYears(1),Child=t.Child }).ToList();
        }
    }
}
