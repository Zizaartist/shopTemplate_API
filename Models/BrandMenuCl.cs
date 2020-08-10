using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    public partial class BrandMenuCl
    {
        public BrandMenuCl()
        {
            ProductCl = new HashSet<ProductCl>();
        }

        public int BrandMenuId { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string UrlImg1 { get; set; }
        public DateTime CreatedDate { get; set; }
        public int BrandId { get; set; }

        public virtual BrandCl Brand { get; set; }
        public virtual ICollection<ProductCl> ProductCl { get; set; }
    }
}
