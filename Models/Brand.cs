using Click.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ApiClick.Models.ArrayModels;
using ApiClick.Models.EnumModels;
using ApiClick.StaticValues;
using System.Diagnostics;

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
        [Required]
        public Category Category { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string BrandName { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string Description { get; set; }
        /// <summary>
        /// Определяет видимость бренда для пользователей
        /// </summary>
        [Required]
        public bool Available { get; set; }
        [Required]
        public bool HasDiscounts { get; set; } //Изменяется при каждом изменении параметра скидки у product
        [Required]
        public int PointsPercentage { get; set; }
        //Документация
        [Required, MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string OfficialName { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string OGRN { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string INN { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string LegalAddress { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Executor { get; set; }
        [Required]
        public Decimal DeliveryPrice { get; set; }
        [Required]
        public Decimal MinimalPrice { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }

        //Nullable
        public int? ImgLogoId { get; set; }
        public int? ImgBannerId { get; set; }
        public int? CertificateId { get; set; }
        public string? DeliveryTime { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string? Contact { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string? Address { get; set; }
        public float? Rating { get; set; } //null if no reviews
        [MaxLength(ModelLengths.LENGTH_MAX)]
        public string? Rules { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("ImgLogoId")]
        public virtual Image ImgLogo { get; set; }
        [ForeignKey("ImgBannerId")]
        public virtual Image ImgBanner { get; set; }
        [ForeignKey("CertificateId")]
        public virtual Image Certificate { get; set; }
        [NotMapped]
        public virtual ICollection<BrandMenu> BrandMenus { get; set; }
        [NotMapped]
        public virtual ICollection<PaymentMethodsListElement> PaymentMethodsListElements { get; set; }
        [NotMapped]
        public virtual ICollection<HashtagsListElement> HashtagsListElements { get; set; }
        [NotMapped]
        public virtual ICollection<ScheduleListElement> ScheduleListElements { get; set; }

        [NotMapped]
        public ICollection<PaymentMethod> PaymentMethods;
        [NotMapped]
        public ICollection<Hashtag> Hashtags;

        /// <summary>
        /// Проверяет валидность модели, полученной от клиента
        /// </summary>
        public static bool ModelIsValid(Brand _brand) 
        {
            try
            {
                if (_brand == null ||
                    //collections
                    _brand.PaymentMethods == null ||
                    !_brand.PaymentMethods.Any() ||
                    _brand.Hashtags == null ||
                    _brand.ScheduleListElements == null ||
                    !_brand.ScheduleListElements.Any() ||
                    //required
                    string.IsNullOrEmpty(_brand.BrandName) ||
                    string.IsNullOrEmpty(_brand.Description) ||
                    _brand.PointsPercentage == default ||
                    string.IsNullOrEmpty(_brand.OfficialName) ||
                    string.IsNullOrEmpty(_brand.OGRN) ||
                    string.IsNullOrEmpty(_brand.INN) ||
                    string.IsNullOrEmpty(_brand.LegalAddress) ||
                    string.IsNullOrEmpty(_brand.Executor) ||
                    _brand.DeliveryPrice == default ||
                    _brand.MinimalPrice == default ||
                    //chunky
                    _brand.ScheduleListElements.GroupBy(e => e.DayOfWeek).Any(e => e.Count() > 1)) //Нет дубликатов дней
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex) 
            {
                Debug.WriteLine($"Ошибка при проверке данных бренда - {_ex}");
                return false;
            }
        }
    }
}
