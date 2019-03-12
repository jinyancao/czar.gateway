using Czar.Rpc.Attributes;
using Czar.Rpc.Exceptions;
using Czar.Rpc.Metadata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Czar.Rpc.Common
{
    /// <summary>
    /// 测试Rpc实体
    /// </summary>
    [BusinessExceptionInterceptor]
    [CzarRpc("Demo.Rpc.Hello")]
    public interface IHelloRpc: IRpcBaseService
    {
        string Hello(int no, string name);

        void HelloHolder(int no, out string name);

        Task<string> HelloTask(int no, string name);

        ValueTask<string> HelloValueTask(int no, string name);

        [CzarOneway]
        Task HelloOneway(int no, string name);

        Task TestBusinessExceptionInterceptor();

        DemoModel HelloModel(int D1, string D2, DateTime D3);

        Task<DemoModel> HelloModelAsync(int D1, string D2, DateTime D3);

        DemoModel HelloSendModel(DemoModel model);

        DemoModel HelloSendModelParm(string name,DemoModel model);

        List<DemoModel> HelloSendModelList(List<DemoModel> model);
    }
}
