using Click.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ApiClick.Models.ArrayModels;
using ApiClick.Models.EnumModels;
using ApiClick.StaticValues;
using DayOfWeek = ApiClick.Models.EnumModels.DayOfWeek;

namespace ApiClick.Models
{
    public partial class Brand
    {
        public Brand() 
        {
            BrandMenus = new List<BrandMenu>();
            Hashtags = new List<Hashtag>();
            ScheduleListElements = new List<ScheduleListElement>();

            PaymentMethods = new List<PaymentMethod>();
        }

        //Not nullable
        [Key]
        public int BrandId { get; set; }
        public Category Category { get; set; }
        public int UserId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string BrandName { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string Description { get; set; }
        /// <summary>
        /// Определяет видимость бренда для пользователей
        /// </summary>
        public bool Available { get; set; }
        public bool HasDiscounts { get; set; } //Изменяется при каждом изменении параметра скидки у product

        //Nullable
        public int? ImgLogoId { get; set; }
        public int? ImgBannerId { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Contact1 { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)] 
        public string Contact2 { get; set; } //Биполярка блин, то хотят 2 поля, то не хотят
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Address { get; set; }
        public TimeSpan? DeliveryTimeFrom { get; set; }
        public TimeSpan? DeliveryTimeTo { get; set; }
        public float? Rating { get; set; } //null if no reviews
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string Rules { get; set; }
        public Decimal? MinimalPrice { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("ImgLogoId")]
        public virtual Image ImgLogo { get; set; }
        [ForeignKey("ImgBannerId")]
        public virtual Image ImgBanner { get; set; }
        [NotMapped]
        public virtual ICollection<BrandMenu> BrandMenus { get; set; }
        [NotMapped]
        public virtual ICollection<PaymentMethodsListElement> PaymentMethodsListElements { get; set; }
        [NotMapped]
        public virtual ICollection<HashtagsListElement> HashtagsListElements { get; set; }
        [NotMapped]
        public ICollection<ScheduleListElement> ScheduleListElements { get; set; }

        public ICollection<PaymentMethod> PaymentMethods;
        public ICollection<Hashtag> Hashtags;
    }
}
