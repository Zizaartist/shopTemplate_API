using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    [Serializable]
    public partial class UserCl
    {
        public UserCl()
        {
            BrandCl = new HashSet<BrandCl>();
            MessageCl = new HashSet<MessageCl>();
            OrdersCl = new HashSet<OrdersCl>();
        }

        public int UserId { get; set; }
        public int CategoryId { get; set; } //?
        public string Phone { get; set; }
        public string Pasword { get; set; } //to Password
        public string Name { get; set; } //to Login
        public string Street { get; set; }
        public int? House { get; set; }
        public int? Padik { get; set; }
        public int? Etash { get; set; }
        public int? Kv { get; set; }
        public DateTime CreatedDate { get; set; }
        public int role { get; set; } //to Role

        public virtual ICollection<BrandCl> BrandCl { get; set; } //to Brands
        public virtual ICollection<MessageCl> MessageCl { get; set; } //to Messages
        public virtual ICollection<OrdersCl> OrdersCl { get; set; } //to Orders
    }
}
