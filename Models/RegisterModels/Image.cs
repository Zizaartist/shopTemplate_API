using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, отвечающая за хранение пути и владельца изображения
    /// </summary>
    public partial class Image
    {
        [Key]
        public int ImageId { get; set; }
        public int UserId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string Path { get; set; }

        //Навигационные свойства
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
