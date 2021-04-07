using ApiClick.Models.EnumModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class BrandPaymentMethod
    {
        public int BrandPaymentMethodId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public int BrandId { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }
    }
}
