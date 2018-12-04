using Czar.Gateway.Authentication;
using Czar.Gateway.Cache;
using Czar.Gateway.Configuration;
using Czar.Gateway.Stores.MySql;
using Czar.Gateway.Stores.SqlServer;
using Czar.Gateway.RateLimit;
using Czar.Gateway.Responder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.Cache;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.DependencyInjection;
using Ocelot.Responder;
using System;

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
            builder.Services.Configure(option);
            //配置信息
            builder.Services.AddSingleton(
                resolver => resolver.GetRequiredService<IOptions<CzarOcelotConfiguration>>().Value);
            //配置文件仓储注入
            builder.Services.AddSingleton<IFileConfigurationRepository, SqlServerFileConfigurationRepository>();
            builder.Services.AddSingleton<IClientAuthenticationRepository, SqlServerClientAuthenticationRepository>();
            builder.Services.AddSingleton<IClientRateLimitRepository, SqlServerClientRateLimitRepository>();
            //注册后端服务
            builder.Services.AddHostedService<DbConfigurationPoller>();
            //使用Redis重写缓存
            builder.Services.AddSingleton<IOcelotCache<FileConfiguration>, InRedisCache<FileConfiguration>>();
            builder.Services.AddSingleton<IOcelotCache<CachedResponse>, InRedisCache<CachedResponse>>();
            builder.Services.AddSingleton<IInternalConfigurationRepository, RedisInternalConfigurationRepository>();
            builder.Services.AddSingleton<IOcelotCache<ClientRoleModel>, InRedisCache<ClientRoleModel>>();
            builder.Services.AddSingleton<IOcelotCache<RateLimitRuleModel>, InRedisCache<RateLimitRuleModel>>();
            builder.Services.AddSingleton<IOcelotCache<CzarClientRateLimitCounter?>, InRedisCache<CzarClientRateLimitCounter?>>();
            //注入授权
            builder.Services.AddSingleton<ICzarAuthenticationProcessor, CzarAuthenticationProcessor>();
            //注入限流实现
            builder.Services.AddSingleton<IClientRateLimitProcessor, CzarClientRateLimitProcessor>();

            //重写错误状态码
            builder.Services.AddSingleton<IErrorsToHttpStatusCodeMapper, CzarErrorsToHttpStatusCodeMapper>();
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
            return builder;
        }
    }
}
