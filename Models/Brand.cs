using Click.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ApiClick.Models.ArrayModels;
using ApiClick.Models.EnumModels;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    public partial class Brand
    {
        public Brand() 
        {
            BrandMenus = new List<BrandMenu>();
            Hashtags = new List<Hashtag>();
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
        [Required, MaxLength(ModelLengths.LENGTH_MAX)]
        public string DescriptionMax { get; set; }
        /// <summary>
        /// Определяет видимость бренда для пользователей
        /// </summary>
        public bool Available { get; set; }
        public string DeliveryTime { get; set; }

        //Nullable
        public int? ImgLogoId { get; set; }
        public int? ImgBannerId { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Contact { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Address { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public float? Rating { get; set; } //null if no reviews
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string Rules { get; set; }
        

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
        public ICollection<PaymentMethod> PaymentMethods { get; set; }
        [NotMapped]
        public ICollection<Hashtag> Hashtags { get; set; }
    }
}
