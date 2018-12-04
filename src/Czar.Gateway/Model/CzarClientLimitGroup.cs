namespace Czar.Gateway.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CzarClientLimitGroup
    {
        [Key]
        public int ClientLimitGroupId { get; set; }

        public int? Id { get; set; }

        public int? LimitGroupId { get; set; }
    }
}
