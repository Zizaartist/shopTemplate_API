using Newtonsoft.Json;
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
            BrandHashtags = new HashSet<BrandHashtag>();
        }

        public int HashTagId { get; set; }
        public string HashTagName { get; set; }
        [JsonIgnore]
        public Kind Kind { get; set; }

        [JsonIgnore]
        public ICollection<BrandHashtag> BrandHashtags { get; set; }
    }
}
