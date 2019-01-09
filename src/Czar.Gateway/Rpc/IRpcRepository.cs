using Czar.Rpc.Message;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czar.Gateway.Rpc
{
    public interface IRpcRepository
    {
        /// <summary>
        /// 根据模板地址获取RPC请求方法
        /// </summary>
        /// <param name="UpUrl">上游模板</param>
        /// <returns></returns>
        Task<RemoteInvokeMessage> GetRemoteMethodAsync(string UpUrl);
    }
}
