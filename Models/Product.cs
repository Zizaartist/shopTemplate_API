using ApiClick.Models.EnumModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные о продукте, который является частью меню
    /// </summary>
    public class Product
    {
        //Not nullable
        [Key]
        public int ProductId { get; set; }
        [Required]
        public Decimal Price { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string ProductName { get; set; }

        //Nullable
        public int? BrandMenuId { get; set; }
        public int? PriceDiscount { get; set; }
        public int? ImgId { get; set; }
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("BrandMenuId")]
        public virtual BrandMenu BrandMenu { get; set; }
        [ForeignKey("ImgId")]
        public virtual Image Image { get; set; }
    }
}
