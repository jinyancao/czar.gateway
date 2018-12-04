using System.Collections.Generic;

namespace Czar.Gateway.RateLimit
{
    public class RateLimitRuleModel
    {
        /// <summary>
        /// 是否启用限流
        /// </summary>
        public bool RateLimit { get; set; }

        /// <summary>
        /// 限流配置信息
        /// </summary>
        public List<CzarClientRateLimitOptions> rateLimitOptions { get; set; }
    }
}
