using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.AspNetCore.Mvc;

namespace Czar.Sample.TestApi.Controllers
{
    public struct CzarClientRateLimitCounter
    {
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

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value11", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            // throw new Exception("测试异常");
            //RedisHelper.Set("2132", id);
            //RedisHelper.Publish("CzarGateway1-eff-dsf", $"測試{id}");
            return $"return {id}";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
