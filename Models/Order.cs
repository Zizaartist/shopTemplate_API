using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
            PointRegister = new HashSet<PointRegister>();
        }

        public int OrderId { get; set; }
        public int OrdererId { get; set; }
        public int Type { get; set; }
        public int PaymentMethod { get; set; }
        public int OrderStatus { get; set; }
        public bool? PointsUsed { get; set; }
        public bool? Delivery { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual User Orderer { get; set; }
        public virtual OrderInfo OrderInfo { get; set; }
        public virtual Review Review { get; set; }
        public virtual WaterOrder WaterOrder { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
        public virtual ICollection<PointRegister> PointRegister { get; set; }
    }
}
