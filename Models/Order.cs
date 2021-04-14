﻿using ApiClick.Models.EnumModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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
        public PaymentMethod PaymentMethod { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public bool? PointsUsed { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public string Orderer { get; set; }

        public virtual OrderInfo OrderInfo { get; set; }
        [JsonIgnore]
        public Review Review { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [JsonIgnore]
        public virtual ICollection<PointRegister> PointRegisters { get; set; }

        [NotMapped]
        public bool? Delivery { get; set; } //Получаем от клиента

        //Регистр, повлиявший на стоимость заказа, потому сделал отдельный доступ к нему
        [NotMapped]
        [JsonIgnore]
        public PointRegister PointRegister
        {
            get 
            {
                return PointRegisters?.FirstOrDefault(pr => pr.Sender == Orderer);
            }
        }

        [NotMapped]
        public decimal Sum { get; set; }
    }
}
