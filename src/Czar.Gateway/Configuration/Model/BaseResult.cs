using System;
using System.Collections.Generic;
using System.Text;

namespace Czar.Gateway.Configuration
{
    /// <summary>
    /// 金焰的世界
    /// 2018-12-10
    /// 信息输出基类
    /// </summary>
    public class BaseResult
    {
        public BaseResult(int _errcode,string _errmsg)
        {
            errcode = _errcode;
            errmsg = _errmsg;
        }
        public BaseResult()
        {

        }
        /// <summary>
        /// 错误类型标识
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误类型说明
        /// </summary>
        public string errmsg { get; set; }
    }

    /// <summary>
    /// 金焰的世界
    /// 2018-12-10
    /// 默认成功结果
    /// </summary>
    public class SuccessResult : BaseResult
    {
        public SuccessResult() : base(0, "成功")
        {

        }
    }
}
