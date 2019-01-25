using Czar.Gateway.Configuration;
using Czar.Gateway.Model;
using Dapper;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using Ocelot.Responses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Czar.Gateway.Stores.SqlServer
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-11
    /// 使用SqlServer来实现配置文件仓储接口
    /// </summary>
    public class SqlServerFileConfigurationRepository : IFileConfigurationRepository
    {
        private readonly CzarOcelotConfiguration _option;
        public SqlServerFileConfigurationRepository(CzarOcelotConfiguration option)
        {
            _option = option;
        }

        /// <summary>
        /// 从数据库中获取配置信息
        /// </summary>
        /// <returns></returns>
        public async Task<Response<FileConfiguration>> Get()
        {
            #region 提取配置信息
            var file = new FileConfiguration();
            //提取默认启用的路由配置信息
            string glbsql = "select * from AhphGlobalConfiguration where IsDefault=1 and InfoStatus=1";
            //提取全局配置信息
            using (var connection = new SqlConnection(_option.DbConnectionStrings))
            {
                var result = await connection.QueryFirstOrDefaultAsync<CzarGlobalConfiguration>(glbsql);
                if (result != null)
                {
                    var glb = new FileGlobalConfiguration();
                    //赋值全局信息
                    glb.BaseUrl = result.BaseUrl;
                    glb.DownstreamScheme = result.DownstreamScheme;
                    glb.RequestIdKey = result.RequestIdKey;
                    //glb.HttpHandlerOptions = result.HttpHandlerOptions?.ToObject<FileHttpHandlerOptions>();
                    //glb.LoadBalancerOptions = result.LoadBalancerOptions?.ToObject<FileLoadBalancerOptions>();
                    //glb.QoSOptions = result.QoSOptions?.ToObject<FileQoSOptions>();
                    //glb.ServiceDiscoveryProvider = result.ServiceDiscoveryProvider?.ToObject<FileServiceDiscoveryProvider>();
                    if (!String.IsNullOrEmpty(result.HttpHandlerOptions))
                    {
                        glb.HttpHandlerOptions = result.HttpHandlerOptions.ToObject<FileHttpHandlerOptions>();
                    }
                    if (!String.IsNullOrEmpty(result.LoadBalancerOptions))
                    {
                        glb.LoadBalancerOptions = result.LoadBalancerOptions.ToObject<FileLoadBalancerOptions>();
                    }
                    if (!String.IsNullOrEmpty(result.QoSOptions))
                    {
                        glb.QoSOptions = result.QoSOptions.ToObject<FileQoSOptions>();
                    }
                    if (!String.IsNullOrEmpty(result.ServiceDiscoveryProvider))
                    {
                        glb.ServiceDiscoveryProvider = result.ServiceDiscoveryProvider.ToObject<FileServiceDiscoveryProvider>();
                    }
                    file.GlobalConfiguration = glb;

                    //提取所有路由信息
                    string routesql = "select T2.* from AhphConfigReRoutes T1 inner join AhphReRoute T2 on T1.ReRouteId=T2.ReRouteId where AhphId=@AhphId and InfoStatus=1";
                    var routeresult = (await connection.QueryAsync<CzarReRoute>(routesql, new { result.AhphId }))?.AsList();
                    if (routeresult != null && routeresult.Count > 0)
                    {
                        var reroutelist = new List<FileReRoute>();
                        foreach (var model in routeresult)
                        {
                            var m = new FileReRoute();
                            //m.AuthenticationOptions = model.AuthenticationOptions?.ToObject<FileAuthenticationOptions>();
                            //m.FileCacheOptions = model.CacheOptions?.ToObject<FileCacheOptions>();
                            //m.DelegatingHandlers = model.DelegatingHandlers?.ToObject<List<string>>();
                            //m.LoadBalancerOptions = model.LoadBalancerOptions?.ToObject<FileLoadBalancerOptions>();
                            //m.QoSOptions = model.QoSOptions?.ToObject<FileQoSOptions>();
                            //m.DownstreamHostAndPorts = model.DownstreamHostAndPorts?.ToObject<List<FileHostAndPort>>();
                            if (!String.IsNullOrEmpty(model.AuthenticationOptions))
                            {
                                m.AuthenticationOptions = model.AuthenticationOptions.ToObject<FileAuthenticationOptions>();
                            }
                            if (!String.IsNullOrEmpty(model.CacheOptions))
                            {
                                m.FileCacheOptions = model.CacheOptions.ToObject<FileCacheOptions>();
                            }
                            if (!String.IsNullOrEmpty(model.DelegatingHandlers))
                            {
                                m.DelegatingHandlers = model.DelegatingHandlers.ToObject<List<string>>();
                            }
                            if (!String.IsNullOrEmpty(model.LoadBalancerOptions))
                            {
                                m.LoadBalancerOptions = model.LoadBalancerOptions.ToObject<FileLoadBalancerOptions>();
                            }
                            if (!String.IsNullOrEmpty(model.QoSOptions))
                            {
                                m.QoSOptions = model.QoSOptions.ToObject<FileQoSOptions>();
                            }
                            if (!String.IsNullOrEmpty(model.DownstreamHostAndPorts))
                            {
                                m.DownstreamHostAndPorts = model.DownstreamHostAndPorts.ToObject<List<FileHostAndPort>>();
                            }
                            //开始赋值
                            m.DownstreamPathTemplate = model.DownstreamPathTemplate;
                            m.DownstreamScheme = model.DownstreamScheme;
                            m.Key = model.RequestIdKey;
                            m.Priority = model.Priority ?? 0;
                            m.RequestIdKey = model.RequestIdKey;
                            m.ServiceName = model.ServiceName;
                            m.UpstreamHost = model.UpstreamHost;
                            m.UpstreamHttpMethod = model.UpstreamHttpMethod?.ToObject<List<string>>();
                            m.UpstreamPathTemplate = model.UpstreamPathTemplate;
                            reroutelist.Add(m);
                        }
                        file.ReRoutes = reroutelist;
                    }
                }
                else
                {
                    throw new Exception("未监测到任何可用的配置信息");
                }
            }
            #endregion
            if (file.ReRoutes == null || file.ReRoutes.Count == 0)
            {
                return new OkResponse<FileConfiguration>(null);
            }
            return new OkResponse<FileConfiguration>(file);
        }

        /// <summary>
        /// 更新数据库中的配置信息
        /// </summary>
        /// <param name="fileConfiguration"></param>
        /// <returns></returns>
        public async Task<Response> Set(FileConfiguration fileConfiguration)
        {
            using (var con = new SqlConnection(_option.DbConnectionStrings))
            {
                var global = fileConfiguration?.GlobalConfiguration;
                if (global != null && !string.IsNullOrEmpty(global.RequestIdKey))
                {
                    var cmd = "UPDATE AhphGlobalConfiguration SET BaseUrl=@BaseUrl,DownstreamScheme=@DownstreamScheme,ServiceDiscoveryProvider=@ServiceDiscoveryProvider,LoadBalancerOptions=@LoadBalancerOptions,HttpHandlerOptions=@HttpHandlerOptions,QoSOptions=@QoSOptions WHERE RequestIdKey=@RequestIdKey";
                    var result = await con.ExecuteAsync(cmd, new
                    {
                        global.BaseUrl,
                        global.DownstreamScheme,
                        ServiceDiscoveryProvider = global.ServiceDiscoveryProvider.ToJson(),
                        LoadBalancerOptions = global.LoadBalancerOptions.ToJson(),
                        HttpHandlerOptions = global.HttpHandlerOptions.ToJson(),
                        QoSOptions = global.QoSOptions.ToJson(),
                        global.RequestIdKey
                    }, null, null, CommandType.Text);
                }
                var reRoutes = fileConfiguration.ReRoutes;
                if (reRoutes != null && reRoutes.Count > 0)
                {
                    foreach (var item in reRoutes)
                    {
                        var cmd = @"UPDATE AhphReRoute SET UpstreamPathTemplate=@UpstreamPathTemplate,UpstreamHttpMethod=@UpstreamHttpMethod,UpstreamHost=@UpstreamHost,DownstreamScheme=@DownstreamScheme,DownstreamPathTemplate=@DownstreamPathTemplate,
  DownstreamHostAndPorts=@DownstreamHostAndPorts,AuthenticationOptions=@AuthenticationOptions,CacheOptions=@CacheOptions,LoadBalancerOptions=@LoadBalancerOptions,QoSOptions=@QoSOptions,DelegatingHandlers=@DelegatingHandlers,ServiceName=@ServiceName WHERE RequestIdKey=@RequestIdKey";
                        var result = await con.ExecuteAsync(cmd, new
                        {
                            item.UpstreamPathTemplate,
                            item.UpstreamHttpMethod,
                            item.UpstreamHost,
                            item.DownstreamScheme,
                            item.DownstreamPathTemplate,
                            DownstreamHostAndPorts = item.DownstreamHostAndPorts.ToJson(),
                            AuthenticationOptions = item.AuthenticationOptions.ToJson(),
                            CacheOptions = item.FileCacheOptions.ToJson(),
                            LoadBalancerOptions = item.LoadBalancerOptions.ToJson(),
                            QoSOptions = item.QoSOptions.ToJson(),
                            DelegatingHandlers = item.DelegatingHandlers.ToJson(),
                            item.ServiceName,
                            item.RequestIdKey
                        }, null, null, CommandType.Text);
                    }
                }
            }


            return await Task.FromResult<Response>(new OkResponse());
        }
    }
}
