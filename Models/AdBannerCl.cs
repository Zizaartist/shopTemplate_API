using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель рекламного баннера, пока не кликабельного
    /// </summary>
    public class AdBannerCl
    {
        //Non-nullable
        public int AdBannerId { get; set; }
        public int CategoryId { get; set; }
        //Изначальное количество необходимых просмотров (чем больше, тем чаще будет вставляться)
        public int InitialCount { get; set; }
        //Количество оставшихся просмотров
        public int ViewCount { get; set; }

        //Nullable
        public int ImgId { get; set; }
        public string Text { get; set; }

        //Сам, собственно, контент
        public virtual ImageCl Image { get; set; }
        public virtual CategoryCl Category { get; set; }
    }
}
