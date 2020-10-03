using ApiClick.Models.EnumModels;
using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные о продукте, который является частью меню
    /// </summary>
    public partial class ProductCl
    {
        //Not nullable
        public int ProductId { get; set; }
        public int BrandMenuId { get; set; }
        public Decimal Price { get; set; }
        public int ImgId { get; set; }

        //Nullable
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int? PriceDiscount { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual BrandMenuCl BrandMenu { get; set; }
        public virtual ImageCl Image { get; set; }
    }
}
