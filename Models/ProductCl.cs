using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    public partial class ProductCl
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UrlImg1 { get; set; } //to ImgId
        public int Price { get; set; }
        public int? PriceDiscount { get; set; }
        public int? BrandMenuIdDiscount { get; set; } //?
        public int? BrandMenuIdNabori { get; set; } //?
        public int? BrandMenuIdRezerv1 { get; set; } //?
        public int? BrandMenuIdRezerv2 { get; set; } //?
        public DateTime CreatedTime { get; set; } //to CreatedDate
        public int BrandMenuId { get; set; }

        public virtual BrandMenuCl BrandMenu { get; set; }
    }
}
