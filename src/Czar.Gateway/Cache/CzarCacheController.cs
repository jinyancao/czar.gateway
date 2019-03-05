using Czar.Gateway.Authentication;
using Czar.Gateway.Configuration;
using Czar.Gateway.RateLimit;
using Czar.Gateway.Rpc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Ocelot.Configuration;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.Repository;
using System;
using System.Threading.Tasks;

namespace Czar.Gateway.Cache
{
    /// <summary>
    /// 提供外部缓存处理接口
    /// </summary>
    [Authorize]
    [Route("CzarCache")]
    public class CzarCacheController : Controller
    {
        private readonly CzarOcelotConfiguration _options;
        private readonly IClientAuthenticationRepository _clientAuthenticationRepository;
        private IFileConfigurationRepository _fileConfigurationRepository;
        private IInternalConfigurationCreator _internalConfigurationCreator;
        private readonly IClientRateLimitRepository _clientRateLimitRepository;
        private readonly IRpcRepository _rpcRepository;
        private readonly IMemoryCache _cache;
        public CzarCacheController(IClientAuthenticationRepository clientAuthenticationRepository, CzarOcelotConfiguration options,
          IFileConfigurationRepository fileConfigurationRepository,
          IInternalConfigurationCreator internalConfigurationCreator,
          IClientRateLimitRepository clientRateLimitRepository,
          IRpcRepository rpcRepository,
          IMemoryCache cache)
        {
            _clientAuthenticationRepository = clientAuthenticationRepository;
            _options = options;
            _fileConfigurationRepository = fileConfigurationRepository;
            _internalConfigurationCreator = internalConfigurationCreator;
            _clientRateLimitRepository = clientRateLimitRepository;
            _rpcRepository = rpcRepository;
            _cache = cache;
        }

        /// <summary>
        /// 更新客户端地址访问授权接口
        /// </summary>
        /// <param name="clientid">客户端ID</param>
        /// <param name="path">请求模板</param>
        /// <returns></returns>
        [HttpPost]
        [Route("ClientRule")]
        public async Task UpdateClientRuleCache(string clientid,string path)
        {
            var region = CzarCacheRegion.AuthenticationRegion;
            var key = CzarOcelotHelper.ComputeCounterKey(region, clientid, "", path);
            key = CzarOcelotHelper.GetKey(_options.RedisOcelotKeyPrefix, region, key);
            var result = await _clientAuthenticationRepository.ClientAuthenticationAsync(clientid, path);
            var data = new ClientRoleModel() { CacheTime = DateTime.Now, Role = result };
            if (_options.ClusterEnvironment)
            {
                RedisHelper.Set(key, data); //加入redis缓存
                RedisHelper.Publish(key, data.ToJson()); //发布事件
            }
            else
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 更新网关配置路由信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("InternalConfiguration")]
        public async Task UpdateInternalConfigurationCache()
        {
            var key = CzarCacheRegion.InternalConfigurationRegion;
            key = CzarOcelotHelper.GetKey(_options.RedisOcelotKeyPrefix, "", key);
            var fileconfig = await _fileConfigurationRepository.Get();
            var internalConfig = await _internalConfigurationCreator.Create(fileconfig.Data);
            var config = (InternalConfiguration)internalConfig.Data;
            if (_options.ClusterEnvironment)
            {
                RedisHelper.Set(key, config); //加入redis缓存
                RedisHelper.Publish(key, config.ToJson()); //发布事件
            }
            else
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 删除路由配合的缓存信息
        /// </summary>
        /// <param name="region">区域</param>
        /// <param name="downurl">下端路由</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Response")]
        public async Task DeleteResponseCache(string region,string downurl)
        {
            var key = CzarOcelotHelper.GetKey(_options.RedisOcelotKeyPrefix, region, downurl);
            if (_options.ClusterEnvironment)
            {
                await RedisHelper.DelAsync(key);
                RedisHelper.Publish(key, "");//发布时间
            }
            else
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 更新客户端限流规则缓存
        /// </summary>
        /// <param name="clientid">客户端ID</param>
        /// <param name="path">路由模板</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RateLimitRule")]
        public async Task UpdateRateLimitRuleCache(string clientid, string path)
        {
            var region = CzarCacheRegion.RateLimitRuleModelRegion;
            var key = clientid + path;
            key = CzarOcelotHelper.GetKey(_options.RedisOcelotKeyPrefix, region, key);
            var result = await _clientRateLimitRepository.CheckClientRateLimitAsync(clientid, path);
            var data = new RateLimitRuleModel() { RateLimit = result.RateLimit, rateLimitOptions = result.rateLimitOptions };
            if (_options.ClusterEnvironment)
            {
                RedisHelper.Set(key, data); //加入redis缓存
                RedisHelper.Publish(key, data.ToJson()); //发布事件
            }
            else
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 更新客户端是否开启限流缓存
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ClientRole")]
        public async Task UpdateClientRoleCache(string path)
        {
            var region = CzarCacheRegion.ClientRoleModelRegion;
            var key = path;
            key = CzarOcelotHelper.GetKey(_options.RedisOcelotKeyPrefix, region, key);
            var result = await _clientRateLimitRepository.CheckReRouteRuleAsync(path);
            var data = new ClientRoleModel() { CacheTime = DateTime.Now, Role = result };
            if (_options.ClusterEnvironment)
            {
                RedisHelper.Set(key, data); //加入redis缓存
                RedisHelper.Publish(key, data.ToJson()); //发布事件
            }
            else
            {
                _cache.Remove(key);
            }
        }

        /// <summary>
        /// 更新呢客户端路由白名单缓存
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ClientReRouteWhiteList")]
        public async Task UpdateClientReRouteWhiteListCache(string clientid, string path)
        {
            var region = CzarCacheRegion.ClientReRouteWhiteListRegion;
            var key = clientid + path;
            key = CzarOcelotHelper.GetKey(_options.RedisOcelotKeyPrefix, region, key);
            var result = await _clientRateLimitRepository.CheckClientReRouteWhiteListAsync(clientid, path);
            var data = new ClientRoleModel() { CacheTime = DateTime.Now, Role = result };
            if (_options.ClusterEnvironment)
            {
                RedisHelper.Set(key, data); //加入redis缓存
                RedisHelper.Publish(key, data.ToJson()); //发布事件
            }
            else
            {
                _cache.Remove(key);
            }
        }

        [HttpPost]
        [Route("Rpc")]
        public async Task UpdateRpcCache(string UpUrl)
        {
            var region = CzarCacheRegion.RemoteInvokeMessageRegion;
            var key = UpUrl;
            key = CzarOcelotHelper.GetKey(_options.RedisOcelotKeyPrefix, region, key);
            var result = await _rpcRepository.GetRemoteMethodAsync(UpUrl);
            if (_options.ClusterEnvironment)
            {
                RedisHelper.Set(key, result); //加入redis缓存
                RedisHelper.Publish(key, result.ToJson()); //发布事件
            }
            else
            {
                _cache.Remove(key);
            }
        }
    }
}
