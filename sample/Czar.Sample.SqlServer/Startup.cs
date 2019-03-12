using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.Extensions.Reflection;
using Czar.Gateway.Middleware;
using Czar.Rpc.Clients;
using Czar.Rpc.Codec;
using Czar.Rpc.Configurations;
using Czar.Rpc.DotNetty.Extensions;
using Czar.Rpc.DotNetty.Tcp;
using Czar.Rpc.Extensions;
using DotNetty.Buffers;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.Administration;
using Ocelot.DependencyInjection;

namespace Czar.Sample.SqlServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authenticationProviderKey = "TestKey";
            Action<IdentityServerAuthenticationOptions> gatewayoptions = o =>
            {
                o.Authority = "http://localhost:6611";
                o.ApiName = "gateway";
                o.RequireHttpsMetadata = false;
            };

            services.AddAuthentication()
                .AddIdentityServerAuthentication(authenticationProviderKey, gatewayoptions);

            Action<IdentityServerAuthenticationOptions> options = o =>
            {
                o.Authority = "http://localhost:6611"; //IdentityServer地址
                o.RequireHttpsMetadata = false;
                o.ApiName = "gateway_admin"; //网关管理的名称，对应的为客户端授权的scope
            };
            services.AddCzarOcelot(option =>
            {
                option.RedisOcelotKeyPrefix = "CzarGateway1";
                option.DbConnectionStrings = "Server=.;Database=Ctr_AuthPlatform;User ID=sa;Password=bl123456;";
                option.RedisConnectionString = "192.168.1.111:6379,password=bl123456,defaultDatabase=0,poolsize=50,ssl=false,writeBuffer=10240,connectTimeout=1000,connectRetry=1;"
                ;
                option.ClientAuthorization = true;
                option.ClientRateLimit = true;
            })
            //.UseMySql()
            .AddAdministration("/CzarOcelot", options);
            #region 注入Rpc相关的服务
            services.AddLibuvTcpClient(Configuration);
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCzarOcelot().Wait();
        }
    }
}
