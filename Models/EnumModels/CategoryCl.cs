using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    /// <summary>
    /// Enum модель, отвечающая за хранение категорий
    /// </summary>
    public partial class CategoryCl
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
