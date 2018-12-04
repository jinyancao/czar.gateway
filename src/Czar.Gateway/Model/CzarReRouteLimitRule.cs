namespace Czar.Gateway.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    public partial class CzarReRouteLimitRule
    {
        [Key]
        public int ReRouteLimitId { get; set; }

        public int? RuleId { get; set; }

        public int? ReRouteId { get; set; }
    }
}
