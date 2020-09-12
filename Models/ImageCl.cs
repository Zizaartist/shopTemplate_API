using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, отвечающая за хранение пути и владельца изображения
    /// </summary>
    public partial class ImageCl
    {

        public int ImageId { get; set; }
        public int UserId { get; set; }
        public string Path { get; set; }

        public virtual UserCl User { get; set; }

    }
}
