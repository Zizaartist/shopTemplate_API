using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая пользовательские данные
    /// </summary>
    [Serializable]
    public partial class UserCl
    {
        public UserCl()
        {
            Brands = new HashSet<BrandCl>();
            Messages = new HashSet<MessageCl>();
            Orders = new HashSet<OrdersCl>();
        }

        //Not nullable
        public int UserId { get; set; } 
        public string Phone { get; set; }
        public int Role { get; set; }
        public int Points { get; set; }

        //Nullable
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public int? House { get; set; }
        public int? Padik { get; set; }
        public int? Etash { get; set; }
        public int? Kv { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual ICollection<BrandCl> Brands { get; set; }
        public virtual ICollection<MessageCl> Messages { get; set; }
        public virtual ICollection<OrdersCl> Orders { get; set; }
    }
}
