using Click.Models;
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
        public string Rules { get; set; }
        public int? Hashtag1Id { get; set; } 
        public int? Hashtag2Id { get; set; }
        public int? Hashtag3Id { get; set; }
        public int? Hashtag4Id { get; set; }
        public int? Hashtag5Id { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual HashTagCl HashTag1 { get; set; }
        public virtual HashTagCl HashTag2 { get; set; }
        public virtual HashTagCl HashTag3 { get; set; }
        public virtual HashTagCl HashTag4 { get; set; }
        public virtual HashTagCl HashTag5 { get; set; }
        public virtual UserCl User { get; set; }
        public virtual CategoryCl Category { get; set; }
        public virtual ImageCl ImgLogo { get; set; }
        public virtual ImageCl ImgBanner { get; set; }
        public virtual ICollection<BrandMenuCl> BrandMenus { get; set; }
    }
}
