using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class BrandDoc
    {
        [JsonIgnore]
        public int BrandDocsId { get; set; }
        [JsonIgnore]
        public int BrandId { get; set; }
        public string OfficialName { get; set; }
        public string Ogrn { get; set; }
        public string Inn { get; set; }
        public string LegalAddress { get; set; }
        public string Executor { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }
    }
}
