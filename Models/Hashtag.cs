using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class Hashtag
    {
        public Hashtag()
        {
            BrandHashtag = new HashSet<BrandHashtag>();
        }

        public int HashTagId { get; set; }
        public string HashTagName { get; set; }
        public int Category { get; set; }

        public virtual ICollection<BrandHashtag> BrandHashtag { get; set; }
    }
}
