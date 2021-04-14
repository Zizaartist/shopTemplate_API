﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    /// <summary>
    /// Вторичная информация заказа, прилагается к Order
    /// </summary>
    public partial class OrderInfo
    {
        [JsonIgnore]
        public int OrderInfoId { get; set; }
        [JsonIgnore]
        public int OrderId { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string OrdererName { get; set; }
        public string House { get; set; }
        public int? Entrance { get; set; }
        public int? Floor { get; set; }
        public int? Apartment { get; set; }
        public string Commentary { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }
    }
}
