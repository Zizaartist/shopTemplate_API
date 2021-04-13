using Newtonsoft.Json;
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
            AdBanners = new HashSet<AdBanner>();
            BrandHashtags = new HashSet<BrandHashtag>();
            BrandPaymentMethods = new HashSet<BrandPaymentMethod>();
            Categories = new HashSet<Category>();
            Orders = new HashSet<Order>();
            Reports = new HashSet<Report>();
            Reviews = new HashSet<Review>();
            ScheduleListElements = new HashSet<ScheduleListElement>();
        }

        public int BrandId { get; set; }
        public Kind Kind { get; set; }
        public int ExecutorId { get; set; }
        public string BrandName { get; set; }
        public bool Available { get; set; }
        public bool HasDiscounts { get; set; }
        public int PointsPercentage { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal MinimalPrice { get; set; }
        public float? Rating { get; set; }
        public int ReviewCount { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        public virtual BrandInfo BrandInfo { get; set; }
        public virtual WaterBrand WaterBrand { get; set; }
        [JsonIgnore]
        public Executor Executor { get; set; }
        public BrandDoc BrandDoc { get; set; }
        public virtual ICollection<BrandHashtag> BrandHashtags { get; set; }
        public virtual ICollection<BrandPaymentMethod> BrandPaymentMethods { get; set; }
        public virtual ICollection<ScheduleListElement> ScheduleListElements { get; set; }
        [JsonIgnore]
        public ICollection<AdBanner> AdBanners { get; set; }
        [JsonIgnore]
        public ICollection<Category> Categories { get; set; }
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
        [JsonIgnore]
        public ICollection<Report> Reports { get; set; }
        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; }
    }
}
