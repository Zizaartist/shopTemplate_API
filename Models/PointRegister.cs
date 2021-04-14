using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    /// <summary>
    /// Ведет учет изменений баллов
    /// </summary>
    public partial class PointRegister
    {
        public decimal Points { get; set; }
        public bool TransactionCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public string Receiver { get; set; }
        [JsonIgnore]
        public string Sender { get; set; }
        [JsonIgnore]
        public int PointRegisterId { get; set; }
        [JsonIgnore]
        public int? OrderId { get; set; }

        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}
