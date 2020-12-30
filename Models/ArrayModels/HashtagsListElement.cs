using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Click.Models;

namespace ApiClick.Models.ArrayModels
{
    //Посредник между брендом и хештегами
    public class HashtagsListElement
    {
        [Key]
        public int HashtagsListElementId { get; set; }
        public int HashtagId { get; set; }
        public int? BrandId { get; set; } //Не обязательно для предотвращения капризов
        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
        [ForeignKey("HashtagId")]
        public virtual Hashtag Hashtag { get; set; } //Хранимое значение
    }
}
