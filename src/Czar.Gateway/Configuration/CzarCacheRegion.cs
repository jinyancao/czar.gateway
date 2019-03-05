namespace Czar.Gateway.Configuration
{
    /// <summary>
    /// 缓存所属区域
    /// </summary>
    public class CzarCacheRegion
    {
        /// <summary>
        /// 授权
        /// </summary>
        public const string AuthenticationRegion = "CacheClientAuthentication";

        /// <summary>
        /// 路由配置
        /// </summary>
        public const string FileConfigurationRegion = "CacheFileConfiguration";

        /// <summary>
        /// 内部配置
        /// </summary>
        public const string InternalConfigurationRegion = "CacheInternalConfiguration";

        /// <summary>
        /// 客户端权限
        /// </summary>
        public const string ClientRoleModelRegion = "CacheClientRoleModel";

        /// <summary>
        /// 客户端路由白名单
        /// </summary>
        public const string ClientReRouteWhiteListRegion = "CacheClientReRouteWhiteList";

        /// <summary>
        /// 限流规则
        /// </summary>
        public const string RateLimitRuleModelRegion = "CacheRateLimitRuleModel";

        /// <summary>
        /// Rpc远程调用
        /// </summary>
        public const string RemoteInvokeMessageRegion = "CacheRemoteInvokeMessage";

        /// <summary>
        /// 客户端限流
        /// </summary>
        public const string CzarClientRateLimitCounterRegion = "CacheCzarClientRateLimitCounter";
    }
}
