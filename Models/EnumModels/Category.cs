using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Enum модель, отвечающая за хранение категорий
    /// </summary>
    public partial class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string CategoryName { get; set; }
    }
}
