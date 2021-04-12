using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class WaterRequest
    {
        public int WaterRequestId { get; set; }
        public int WaterOrderId { get; set; }
        [JsonIgnore]
        public int WaterBrandId { get; set; }

        public WaterBrand WaterBrand { get; set; }
        [JsonIgnore]
        public WaterOrder WaterOrder { get; set; }
    }
}
