using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    public partial class BrandCl
    {
        public BrandCl()
        {
            BrandMenus = new HashSet<BrandMenuCl>();
        }

        //Not nullable
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public int ImgLogoId { get; set; }
        public int ImgBannerId { get; set; }
        public string BrandName { get; set; }
        public string Description { get; set; }
        public string DescriptionMax { get; set; }

        //Nullable
        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string WorkTime { get; set; }
        public int? Rating { get; set; } //null if no reviews
        public string Hashtag1 { get; set; } 
        public string Hashtag2 { get; set; }
        public string Hashtag3 { get; set; }
        public string Hashtag4 { get; set; }
        public string Hashtag5 { get; set; }
        public string UrlImg1 { get; set; } //?
        public string UrlImg2 { get; set; } //?
        public string UrlImg3 { get; set; } //?
        public string UrlImg4 { get; set; } //?
        public string UrlImg5 { get; set; } //?
        public DateTime CreatedDate { get; set; }

        public virtual UserCl User { get; set; }
        public virtual CategoryCl Category { get; set; }
        public virtual ImageCl ImgLogo { get; set; }
        public virtual ImageCl ImgBanner { get; set; }
        public virtual ICollection<BrandMenuCl> BrandMenus { get; set; }
    }
}
