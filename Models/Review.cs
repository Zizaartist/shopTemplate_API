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
    public partial class Review
    {
        public Review() 
        {
            Products = new HashSet<string>();
        }

        [JsonIgnore]
        public int ReviewId { get; set; }
        [JsonIgnore]
        public int? SenderId { get; set; }
        public int BrandId { get; set; }
        public int? OrderId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User Sender { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
        [JsonIgnore]
        public Brand Brand { get; set; }

        [NotMapped]
        public ICollection<string> Products { get; set; }
    }
}
