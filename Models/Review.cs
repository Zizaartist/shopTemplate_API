using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int UserId { get; set; }
        public int? OrderId { get; set; }
        public int Rating { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public ICollection<string> Products { get; set; }
    }
}
