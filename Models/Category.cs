using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string CategoryName { get; set; }
        public string Image { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
