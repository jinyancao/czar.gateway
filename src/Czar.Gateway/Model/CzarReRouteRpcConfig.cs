using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Czar.Gateway.Model
{
    public partial class CzarReRouteRpcConfig
    {
        [Key]
        public int RpcId { get; set; }


        public int ReRouteId { get; set; }

        [Required]
        [StringLength(100)]
        public string ServantName { get; set; }

        [Required]
        [StringLength(100)]
        public string FuncName { get; set; }


        public bool IsOneway { get; set; }
    }
}
