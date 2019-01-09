using Czar.Gateway.Configuration;
using Czar.Gateway.Rpc;
using Czar.Rpc.Message;
using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Czar.Gateway.Stores.MySql
{
    /// <summary>
    /// 金焰的世界
    /// 2019-01-03
    /// Rpc相关仓储
    /// </summary>
    public class MySqlRpcRepository : IRpcRepository
    {
        private readonly CzarOcelotConfiguration _option;
        public MySqlRpcRepository(CzarOcelotConfiguration option)
        {
            _option = option;
        }

        /// <summary>
        /// 获取RPC调用方法
        /// </summary>
        /// <param name="UpUrl"></param>
        /// <returns></returns>
        public async Task<RemoteInvokeMessage> GetRemoteMethodAsync(string UpUrl)
        {
            using (var connection = new MySqlConnection(_option.DbConnectionStrings))
            {
                string sql = @"select T4.* from AhphGlobalConfiguration t1 inner join AhphConfigReRoutes T2 on
T1.AhphId=t2.AhphId inner join AhphReRoute T3 on T2.ReRouteId=T3.ReRouteId 
INNER JOIN AhphReRouteRpcConfig T4 ON T3.ReRouteId=T4.ReRouteId
where IsDefault=1 and T1.InfoStatus=1 AND T3.InfoStatus=1 AND UpstreamPathTemplate=@URL";
                var result = await connection.QueryFirstOrDefaultAsync<RemoteInvokeMessage>(sql, new { URL = UpUrl });
                return result;
            }
        }
    }
}
