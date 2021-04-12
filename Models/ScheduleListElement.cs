using ApiClick.Controllers.FrequentlyUsed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    public partial class ScheduleListElement
    {
        [JsonIgnore]
        public int ScheduleListElementId { get; set; }
        [JsonIgnore]
        public int BrandId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        [System.Text.Json.Serialization.JsonConverterAttribute(typeof(TimeSpanConverter))]
        public TimeSpan OpenTime { get; set; }
        [System.Text.Json.Serialization.JsonConverterAttribute(typeof(TimeSpanConverter))]
        public TimeSpan CloseTime { get; set; }

        [JsonIgnore]
        public Brand Brand { get; set; }
    }
}
