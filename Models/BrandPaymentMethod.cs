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
        public int PaymentMethod { get; set; }
        public int? BrandId { get; set; }

        public virtual Brand Brand { get; set; }
    }
}
