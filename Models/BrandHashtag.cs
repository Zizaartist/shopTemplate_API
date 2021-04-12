using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class BrandHashtag
    {
        [JsonIgnore]
        public int BrandHashtagsId { get; set; }
        [JsonIgnore]
        public int BrandId { get; set; }
        public int HashtagId { get; set; }

        public virtual Hashtag Hashtag { get; set; }
        [JsonIgnore]
        public Brand Brand { get; set; }
    }
}
