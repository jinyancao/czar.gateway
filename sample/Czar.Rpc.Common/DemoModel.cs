using System;
using System.Collections.Generic;
using System.Text;

namespace Czar.Rpc.Common
{
    public class DemoModel
    {
        /// <summary>
        /// 测试1
        /// </summary>
        public int T1 { get; set; }

        /// <summary>
        /// 测试2
        /// </summary>
        public string T2 { get; set; }

        /// <summary>
        /// 测试3
        /// </summary>
        public DateTime T3 { get; set; }

        public ChildModel Child { get; set; }
    }

    public class ChildModel
    {
        public string C1 { get; set; }
    }
}
