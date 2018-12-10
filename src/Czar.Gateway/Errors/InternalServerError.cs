using Ocelot.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Czar.Gateway.Errors
{
    /// <summary>
    /// 金焰的世界
    /// 2018-12-10
    /// 服务不可用错误
    /// </summary>
    public class InternalServerError:Error
    {
        public InternalServerError(string message) : base(message, OcelotErrorCode.UnableToCompleteRequestError)
        {

        }
    }
}
