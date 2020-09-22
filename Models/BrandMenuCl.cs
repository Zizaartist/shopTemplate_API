using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные меню бренда
    /// </summary>
    public partial class BrandMenuCl
    {
        public BrandMenuCl()
        {
            Products = new HashSet<ProductCl>();
        }

        //Not nullable
        public int BrandMenuId { get; set; }
        public int BrandId { get; set; }

        //Nullable
        public int ImgId { get; set; }
        public string BrandMenuName { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual BrandCl Brand { get; set; } 
        public virtual ImageCl Image { get; set; }
        public virtual ICollection<ProductCl> Products { get; set; }
    }
}
