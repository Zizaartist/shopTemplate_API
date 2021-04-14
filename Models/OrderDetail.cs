using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class OrderDetail
    {
        public int Count { get; set; }
        //на случай удаления продукта из базы
        public int? ProductId { get; set; } 
        public decimal Price { get; set; }
        [JsonIgnore]
        public int OrderDetailId { get; set; }
        [JsonIgnore]
        public int OrderId { get; set; }

        public virtual Product Product { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }
    }
}
