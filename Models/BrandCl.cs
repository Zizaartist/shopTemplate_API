using System;
using System.Collections.Generic;

namespace ApiClick.Models
{
    public partial class BrandCl
    {
        public BrandCl()
        {
            BrandMenuCl = new HashSet<BrandMenuCl>();
            //add hashsets for images and hashtags
        }

        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; } //to BrandName
        public string Description { get; set; }
        public string DescriptionMax { get; set; }
        public string UrlImgLogo { get; set; } //to ImgLogoId
        public string UrlImgBanner { get; set; } //to ImgBannerId
        public string UrlImg1 { get; set; } //?
        public string UrlImg2 { get; set; } //?
        public string UrlImg3 { get; set; } //?
        public string UrlImg4 { get; set; } //?
        public string UrlImg5 { get; set; } //?
        public string Hashtag1 { get; set; } //?
        public string Hashtag2 { get; set; } //?
        public string Hashtag3 { get; set; } //?
        public string Hashtag4 { get; set; } //?
        public string Hashtag5 { get; set; } //?
        public string Price { get; set; } //?
        public string Contact { get; set; } 
        public string Address { get; set; }
        public string Phone { get; set; } //? isn't it the same as contact
        public string WorkTime { get; set; }
        public int? Likes { get; set; } //?
        public int? DisLike { get; set; } //?
        public int? Rating { get; set; }
        public int? Views { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }

        public virtual UserCl User { get; set; }
        public virtual ICollection<BrandMenuCl> BrandMenuCl { get; set; } //to BrandMenus
    }
}
