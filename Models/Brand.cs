using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class Brand
    {
        public Brand()
        {
            AdBanner = new HashSet<AdBanner>();
            BrandHashtag = new HashSet<BrandHashtag>();
            BrandPaymentMethod = new HashSet<BrandPaymentMethod>();
            Category = new HashSet<Category>();
            Order = new HashSet<Order>();
            Report = new HashSet<Report>();
            Review = new HashSet<Review>();
            ScheduleListElement = new HashSet<ScheduleListElement>();
            WaterRequest = new HashSet<WaterRequest>();
        }

        public int BrandId { get; set; }
        public int Type { get; set; }
        public int UserId { get; set; }
        public string BrandName { get; set; }
        public bool? Available { get; set; }
        public bool HasDiscounts { get; set; }
        public int PointsPercentage { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal MinimalPrice { get; set; }
        public DateTime CreatedDate { get; set; }
        public float? Rating { get; set; }
        public int ReviewCount { get; set; }

        public virtual User User { get; set; }
        public virtual BrandDoc BrandDoc { get; set; }
        public virtual BrandInfo BrandInfo { get; set; }
        public virtual Executor Executor { get; set; }
        public virtual WaterBrand WaterBrand { get; set; }
        public virtual ICollection<AdBanner> AdBanner { get; set; }
        public virtual ICollection<BrandHashtag> BrandHashtag { get; set; }
        public virtual ICollection<BrandPaymentMethod> BrandPaymentMethod { get; set; }
        public virtual ICollection<Category> Category { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<Report> Report { get; set; }
        public virtual ICollection<Review> Review { get; set; }
        public virtual ICollection<ScheduleListElement> ScheduleListElement { get; set; }
        public virtual ICollection<WaterRequest> WaterRequest { get; set; }
    }
}
