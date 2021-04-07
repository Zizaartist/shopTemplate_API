using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class WaterOrder
    {
        public WaterOrder()
        {
            WaterRequests = new HashSet<WaterRequest>();
        }

        public int WaterOrderId { get; set; }
        public int OrderId { get; set; }
        public int Amount { get; set; }
        public decimal? Price { get; set; }
        public bool? Container { get; set; }
        public string DeliveryDate { get; set; }
        public bool Express { get; set; }

        public Order Order { get; set; }
        public ICollection<WaterRequest> WaterRequests { get; set; }
    }
}
