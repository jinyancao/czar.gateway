using Newtonsoft.Json;
using System;

namespace Czar.Gateway.RateLimit
{
    /// <summary>
    /// 金焰的世界
    /// 2018-11-19
    /// 客户端限流计数器
    /// </summary>
    public struct CzarClientRateLimitCounter
    {
        [JsonConstructor]
        public CzarClientRateLimitCounter(DateTime timestamp, long totalRequests)
        {
            Timestamp = timestamp;
            TotalRequests = totalRequests;
        }

        /// <summary>
        /// 最后请求时间
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// 请求总数
        /// </summary>
        public long TotalRequests { get; private set; }
    }
}
