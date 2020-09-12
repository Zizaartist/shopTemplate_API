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
        public int CategoryId { get; set; }
        public int BrandMenuId { get; set; }
        public int ImgId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        //Nullable
        public int? PriceDiscount { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual BrandMenuCl BrandMenu { get; set; }
        public virtual CategoryCl Category { get; set; }
        public virtual ImageCl Image { get; set; }
    }
}
