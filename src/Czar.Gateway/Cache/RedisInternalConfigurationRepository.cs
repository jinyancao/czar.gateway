using Czar.Gateway.Configuration;
using Ocelot.Cache;
using Ocelot.Configuration;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Czar.Gateway.Cache
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-14
    /// 使用redis存储内部配置信息
    /// </summary>
    public class RedisInternalConfigurationRepository : IInternalConfigurationRepository
    {
        private readonly CzarOcelotConfiguration _options;
        private IFileConfigurationRepository _fileConfigurationRepository;
        private IInternalConfigurationCreator _internalConfigurationCreator;
        private readonly IOcelotCache<InternalConfiguration> _ocelotCache;
        public RedisInternalConfigurationRepository(CzarOcelotConfiguration options,IFileConfigurationRepository fileConfigurationRepository, IInternalConfigurationCreator internalConfigurationCreator, IOcelotCache<InternalConfiguration> ocelotCache)
        {
            _fileConfigurationRepository = fileConfigurationRepository;
            _internalConfigurationCreator = internalConfigurationCreator;
            _options = options;
            _ocelotCache = ocelotCache;
        }

        /// <summary>
        /// 设置配置信息
        /// </summary>
        /// <param name="internalConfiguration">配置信息</param>
        /// <returns></returns>
        public Response AddOrReplace(IInternalConfiguration internalConfiguration)
        {
            var key = CzarCacheRegion.InternalConfigurationRegion;
            _ocelotCache.Add(key, (InternalConfiguration)internalConfiguration, TimeSpan.FromSeconds(_options.CzarCacheTime), "");
            return new OkResponse();
        }

        /// <summary>
        /// 从缓存中获取配置信息
        /// </summary>
        /// <returns></returns>
        public Response<IInternalConfiguration> Get()
        {
            var key = CzarCacheRegion.InternalConfigurationRegion;
            var result = _ocelotCache.Get(key, "");
            if (result!=null)
            {
                return new OkResponse<IInternalConfiguration>(result);
            }
            var fileconfig= _fileConfigurationRepository.Get().Result;
            var internalConfig= _internalConfigurationCreator.Create(fileconfig.Data).Result;
            AddOrReplace(internalConfig.Data);
            return new OkResponse<IInternalConfiguration>(internalConfig.Data);
        }
    }
}
