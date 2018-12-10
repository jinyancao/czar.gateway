# Czar.gateway

#### 项目介绍
Czar网关项目，负责网关相关功能扩展及应用,目前支持mysql、sqlserver两种存储方式，已经实现动态路由、认证、授权、限流、缓存等特性，下一步将会增加日志和监控等功能。

#### 博客同步更新地址

<https://www.cnblogs.com/jackcao/>

#### 使用方式

```c#
public void ConfigureServices(IServiceCollection services)
{
    var authenticationProviderKey = "TestKey";
    Action<IdentityServerAuthenticationOptions> gatewayoptions = o =>
    {
        o.Authority = "http://localhost:7777";
        o.ApiName = "gateway";
        o.RequireHttpsMetadata = false;
    };

    services.AddAuthentication()
        .AddIdentityServerAuthentication(authenticationProviderKey, gatewayoptions);

    Action<IdentityServerAuthenticationOptions> options = o =>
    {
        o.Authority = "http://localhost:7777"; //IdentityServer地址
        o.RequireHttpsMetadata = false;
        o.ApiName = "gateway_admin"; //网关管理的名称，对应的为客户端授权的scope
    };
    services.AddOcelot().AddCzarOcelot(option =>
                                       {
                                           option.DbConnectionStrings = Configuration["CzarConfig:DbConnectionStrings"];
                                           option.RedisConnectionStrings = new List<string>() {        Configuration["CzarConfig:RedisConnectionStrings"]
                                                                                              };
                                           //option.EnableTimer = true;//启用定时任务
                                           //option.TimerDelay = 10 * 000;//周期10秒
                                           option.ClientAuthorization = true;
                                           option.ClientRateLimit = true;
                                       })
        //.UseMySql() //使用mysql
        .AddAdministration("/CzarOcelot", options);
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }
    app.UseCzarOcelot().Wait();
}
```



#### 版本更新记录

1. 0.2.0版本更新记录
      初始化项目内容，并统一风格。

2. 0.2.1版本

   修复缓存信息失效后，未从数据库提出最新的配置信息bug。



