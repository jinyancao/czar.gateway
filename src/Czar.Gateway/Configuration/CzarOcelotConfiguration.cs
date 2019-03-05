namespace Czar.Gateway.Configuration
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-11
    /// 自定义配置信息
    /// </summary>
    public class CzarOcelotConfiguration
    {
        /// <summary>
        /// 数据库连接字符串，使用不同数据库时自行修改,默认实现了SQLSERVER
        /// </summary>
        public string DbConnectionStrings { get; set; }

        /// <summary>
        /// 金焰的世界
        /// 2018-11-12
        /// 是否启用定时器，默认不启动
        /// </summary>
        public bool EnableTimer { get; set; } = false;

        /// <summary>
        /// 金焰的世界
        /// 2018-11.12
        /// 定时器周期，单位（毫秒），默认30分总自动更新一次
        /// </summary>
        public int TimerDelay { get; set; } = 30 * 60 * 1000;

        /// <summary>
        /// 金焰的世界
        /// 2018-11-14
        /// Redis连接字符串
        /// </summary>
        public string RedisConnectionString { get; set; }

        /// <summary>
        /// 金焰的世界
        /// 2019-03-03
        /// 配置哨兵或分区时使用
        /// </summary>
        public string[] RedisSentinelOrPartitionConStr { get; set; }

        /// <summary>
        /// 金焰的世界
        /// 2019-03-03
        /// Redis部署方式，默认使用普通方式
        /// </summary>
        public RedisStoreMode RedisStoreMode { get; set; } = RedisStoreMode.Normal;

        /// <summary>
        /// 金焰的计界
        /// 2019-03-03
        /// 做集群缓存同步时使用，会订阅所有正则匹配的事件
        /// </summary>
        public string RedisOcelotKeyPrefix { get; set; } = "CzarOcelot";

        /// <summary>
        /// 金焰的世界
        /// 2019-03-03
        /// 是否启用集群环境，如果非集群环境直接本地缓存+数据库即可
        /// </summary>
        public bool ClusterEnvironment { get; set; } = false;

        /// <summary>
        /// 金焰的世界
        /// 2018-11-15
        /// 是否启用客户端授权,默认不开启
        /// </summary>
        public bool ClientAuthorization { get; set; } = false;

        /// <summary>
        /// 金焰的世界
        /// 2018-11-15
        /// 服务器缓存时间，默认30分钟
        /// </summary>
        public int CzarCacheTime { get; set; } = 1800;
        /// <summary>
        /// 金焰的世界
        /// 2018-11-15
        /// 客户端标识，默认 client_id
        /// </summary>
        public string ClientKey { get; set; } = "client_id";

        /// <summary>
        /// 金焰的世界
        /// 2018-11-18
        /// 是否开启自定义限流，默认不开启
        /// </summary>
        public bool ClientRateLimit { get; set; } = false;
    }
}
