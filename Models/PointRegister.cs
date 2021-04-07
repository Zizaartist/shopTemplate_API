using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class PointRegister
    {
        public int PointRegisterId { get; set; }
        public int? OrderId { get; set; }
        public int? SenderId { get; set; }
        public int ReceiverId { get; set; }
        public decimal Points { get; set; }
        public bool TransactionCompleted { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public Order Order { get; set; }
        [JsonIgnore]
        public User Receiver { get; set; }
        [JsonIgnore]
        public User Sender { get; set; }
    }
}
