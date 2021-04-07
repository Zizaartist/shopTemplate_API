using ApiClick.Models.EnumModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            PointRegisters = new HashSet<PointRegister>();
        }

        public int OrderId { get; set; }
        public int OrdererId { get; set; }
        public Kind Kind { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public bool? PointsUsed { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? BrandId { get; set; }
        public int? PointRegisterId { get; set; }

        public virtual OrderInfo OrderInfo { get; set; }
        public virtual WaterOrder WaterOrder { get; set; }
        public Brand Brand { get; set; }
        public User Orderer { get; set; }
        public Review Review { get; set; }
        public virtual PointRegister PointRegister { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<PointRegister> PointRegisters { get; set; }

        [NotMapped]
        public bool? Delivery { get; set; } //Получаем от клиента
    }
}
