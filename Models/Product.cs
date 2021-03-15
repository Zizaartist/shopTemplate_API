using ApiClick.Models.EnumModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ApiClick.StaticValues;
using System.Diagnostics;

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

        /// <summary>
        /// Проверяет валидность модели, полученной от клиента
        /// </summary>
        public static bool ModelIsValid(Product _product)
        {
            try
            {
                if (_product == null ||
                    //Required
                    _product.Price == default ||
                    string.IsNullOrEmpty(_product.ProductName))
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                Debug.WriteLine($"Ошибка при проверке данных продукции - {_ex}");
                return false;
            }
        }
    }
}
