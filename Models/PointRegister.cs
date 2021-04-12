using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class PointRegister
    {
        [JsonIgnore]
        public int PointRegisterId { get; set; }
        [JsonIgnore]
        public int? OrderId { get; set; }
        public int? SenderId { get; set; }
        public int ReceiverId { get; set; }
        public decimal Points { get; set; }
        public bool TransactionCompleted { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public virtual User Receiver { get; set; }
        [JsonIgnore]
        public virtual User Sender { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}
