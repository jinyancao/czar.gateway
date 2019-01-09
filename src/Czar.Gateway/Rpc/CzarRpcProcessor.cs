using Czar.Gateway.Configuration;
using Czar.Rpc.Message;
using Ocelot.Cache;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czar.Gateway.Rpc
{
    public class CzarRpcProcessor: ICzarRpcProcessor
    {
        private readonly IRpcRepository _rpcRepository;
        private readonly CzarOcelotConfiguration _options;
        private readonly IOcelotCache<RemoteInvokeMessage> _ocelotCache;
        public CzarRpcProcessor(IRpcRepository rpcRepository, CzarOcelotConfiguration options, IOcelotCache<RemoteInvokeMessage> ocelotCache)
        {
            _rpcRepository = rpcRepository;
            _options = options;
            _ocelotCache = ocelotCache;
        }

        /// <summary>
        /// 根据模板地址获取RPC请求方法
        /// </summary>
        /// <param name="UpUrl">上游模板</param>
        /// <returns></returns>
        public async Task<RemoteInvokeMessage> GetRemoteMethodAsync(string UpUrl)
        {
            var region = _options.RedisKeyPrefix + "GetRemoteMethodAsync";
            var key = region + UpUrl;
            var cacheResult = _ocelotCache.Get(key, region);
            if (cacheResult != null)
            {//提取缓存数据
                return cacheResult;
            }
            else
            {
                cacheResult = await _rpcRepository.GetRemoteMethodAsync(UpUrl);
                _ocelotCache.Add(key, cacheResult, TimeSpan.FromSeconds(_options.ClientRateLimitCacheTime), region);
                return cacheResult;
            }
        }
    }
}
