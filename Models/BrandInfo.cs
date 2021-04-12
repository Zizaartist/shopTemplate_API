using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class BrandInfo
    {
        [JsonIgnore]
        public int BrandInfoId { get; set; }
        [JsonIgnore]
        public int BrandId { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
        public string Banner { get; set; }
        public string DeliveryTime { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string DeliveryTerms { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }
    }
}
