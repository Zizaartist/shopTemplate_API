using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class UserInfo
    {
        public string Street { get; set; }
        public string House { get; set; }
        public int? Entrance { get; set; }
        public int? Floor { get; set; }
        public int? Apartment { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public int UserInfoId { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
