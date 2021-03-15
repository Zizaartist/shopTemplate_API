using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая данные меню бренда
    /// </summary>
    public partial class BrandMenu
    {
        public BrandMenu() 
        {
            Products = new List<Product>();
        }

        //Not nullable
        [Key]
        public int BrandMenuId { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string BrandMenuName { get; set; }

        //Nullable
        public int? ImgId { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; } 
        [ForeignKey("ImgId")]
        public virtual Image Image { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        
        /// <summary>
        /// Проверяет валидность модели, полученной от клиента
        /// </summary>
        public static bool ModelIsValid(BrandMenu _brandMenu)
        {
            try
            {
                if (_brandMenu == null ||
                    //Required
                    _brandMenu.BrandId == default ||
                    string.IsNullOrEmpty(_brandMenu.BrandMenuName)) 
                {
                    return false;
                }
                return true;
            }
            catch (Exception _ex)
            {
                Debug.WriteLine($"Ошибка при проверке данных меню - {_ex}");
                return false;
            }
        }
    }
}
