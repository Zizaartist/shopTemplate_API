using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class WaterBrand
    {
        public WaterBrand()
        {
            WaterRequest = new HashSet<WaterRequest>();
        }

        public int WaterBrandId { get; set; }
        public int BrandId { get; set; }
        public decimal WaterPrice { get; set; }
        public decimal? ContainerPrice { get; set; }
        public string Certificate { get; set; }

        public Brand Brand { get; set; }
        public virtual ICollection<WaterRequest> WaterRequest { get; set; }
    }
}
