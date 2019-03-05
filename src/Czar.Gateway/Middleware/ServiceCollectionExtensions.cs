using Czar.Gateway.Authentication;
using Czar.Gateway.Cache;
using Czar.Gateway.Configuration;
using Czar.Gateway.Stores.MySql;
using Czar.Gateway.Stores.SqlServer;
using Czar.Gateway.RateLimit;
using Czar.Gateway.Responder;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Cache;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;
using Ocelot.Responder;
using System;
using Czar.Gateway.Rpc;
using Czar.Rpc.Message;
using Ocelot.Configuration;

namespace Czar.Gateway.Middleware
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-12
    /// 扩展Ocelot实现的自定义的注入
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加默认的注入方式，所有需要传入的参数都是用默认值
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IOcelotBuilder AddCzarOcelot(this IOcelotBuilder builder, Action<CzarOcelotConfiguration> option)
        {
            var options = new CzarOcelotConfiguration();
            builder.Services.AddSingleton(options);
            option?.Invoke(options);
            //配置文件仓储注入
            builder.Services.AddSingleton<IFileConfigurationRepository, SqlServerFileConfigurationRepository>();
            builder.Services.AddSingleton<IClientAuthenticationRepository, SqlServerClientAuthenticationRepository>();
            builder.Services.AddSingleton<IClientRateLimitRepository, SqlServerClientRateLimitRepository>();
            builder.Services.AddSingleton<IRpcRepository, SqlServerRpcRepository>();
            //注册后端服务
            builder.Services.AddHostedService<DbConfigurationPoller>();
            builder.Services.AddMemoryCache(); //添加本地缓存
            #region 启动Redis缓存，并支持普通模式 官方集群模式  哨兵模式 分区模式
            if (options.ClusterEnvironment)
            {
                //默认使用普通模式
                var csredis = new CSRedis.CSRedisClient(options.RedisConnectionString);
                switch (options.RedisStoreMode)
                {
                    case RedisStoreMode.Partition:
                        var NodesIndex = options.RedisSentinelOrPartitionConStr;
                        Func<string, string> nodeRule = null;
                        csredis = new CSRedis.CSRedisClient(nodeRule, options.RedisSentinelOrPartitionConStr);
                        break;
                    case RedisStoreMode.Sentinel:
                        csredis = new CSRedis.CSRedisClient(options.RedisConnectionString, options.RedisSentinelOrPartitionConStr);
                        break;
                }
                //初始化 RedisHelper
                RedisHelper.Initialization(csredis);
            }
            #endregion
            builder.Services.AddSingleton<IOcelotCache<FileConfiguration>, CzarMemoryCache<FileConfiguration>>();
            builder.Services.AddSingleton<IOcelotCache<InternalConfiguration>, CzarMemoryCache<InternalConfiguration>>();
            builder.Services.AddSingleton<IOcelotCache<CachedResponse>, CzarMemoryCache<CachedResponse>>();
            builder.Services.AddSingleton<IInternalConfigurationRepository, RedisInternalConfigurationRepository>();
            builder.Services.AddSingleton<IOcelotCache<ClientRoleModel>, CzarMemoryCache<ClientRoleModel>>();
            builder.Services.AddSingleton<IOcelotCache<RateLimitRuleModel>, CzarMemoryCache<RateLimitRuleModel>>();
            builder.Services.AddSingleton<IOcelotCache<RemoteInvokeMessage>, CzarMemoryCache<RemoteInvokeMessage>>();
            builder.Services.AddSingleton<IOcelotCache<CzarClientRateLimitCounter?>, CzarMemoryCache<CzarClientRateLimitCounter?>>();
            //注入授权
            builder.Services.AddSingleton<ICzarAuthenticationProcessor, CzarAuthenticationProcessor>();
            //注入限流实现
            builder.Services.AddSingleton<IClientRateLimitProcessor, CzarClientRateLimitProcessor>();

            //重写错误状态码
            builder.Services.AddSingleton<IErrorsToHttpStatusCodeMapper, CzarErrorsToHttpStatusCodeMapper>();

            //http输出转换类
            builder.Services.AddSingleton<IHttpResponder, CzarHttpContextResponder>();

            //Rpc应用
            builder.Services.AddSingleton<ICzarRpcProcessor, CzarRpcProcessor>();
            return builder;
        }

        /// <summary>
        /// 扩展使用Mysql存储。
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IOcelotBuilder UseMySql(this IOcelotBuilder builder)
        {
            builder.Services.AddSingleton<IFileConfigurationRepository, MySqlFileConfigurationRepository>();
            builder.Services.AddSingleton<IClientAuthenticationRepository, MySqlClientAuthenticationRepository>();
            builder.Services.AddSingleton<IClientRateLimitRepository, MySqlClientRateLimitRepository>();
            builder.Services.AddSingleton<IRpcRepository, MySqlRpcRepository>();
            return builder;
        }
    }
}
