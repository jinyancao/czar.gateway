namespace Czar.Gateway.Model
{
    using System.ComponentModel.DataAnnotations;

    public partial class CzarConfigReRoutes
    {
        [Key]
        public int CtgRouteId { get; set; }

        public int? AhphId { get; set; }

        public int? ReRouteId { get; set; }
    }
}
