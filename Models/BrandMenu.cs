using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные меню бренда
    /// </summary>
    public partial class BrandMenu
    {
        public BrandMenu() 
        {
            Products = new List<Product>();
        }

        //Not nullable
        [Key]
        public int BrandMenuId { get; set; }

        //Nullable
        public int? BrandId { get; set; }
        public int? ImgId { get; set; }
        [MaxLength(ModelLengths.LENGTH_SMALL)]
        public string BrandMenuName { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } 
        [ForeignKey("ImgId")]
        public virtual Image Image { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
