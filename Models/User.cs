using ApiClick.Models.EnumModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    //Вообще ничего не возвращать кроме UserInfo
    public partial class User
    {
        public User()
        {
            ErrorReports = new HashSet<ErrorReport>();
            Orders = new HashSet<Order>();
            PointRegisterReceivers = new HashSet<PointRegister>();
            PointRegisterSenders = new HashSet<PointRegister>();
            Reviews = new HashSet<Review>();
        }

        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public UserRole UserRole { get; set; }
        public string Phone { get; set; }
        [JsonIgnore]
        public decimal Points { get; set; }
        [JsonIgnore]
        public bool NotificationsEnabled { get; set; }
        [JsonIgnore]
        public string NotificationRegistration { get; set; }
        [JsonIgnore]
        public string DeviceType { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }

        public virtual UserInfo UserInfo { get; set; }
        [JsonIgnore]
        public Executor Executor { get; set; }
        [JsonIgnore]
        public ICollection<ErrorReport> ErrorReports { get; set; }
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }
        [JsonIgnore]
        public ICollection<PointRegister> PointRegisterReceivers { get; set; }
        [JsonIgnore]
        public ICollection<PointRegister> PointRegisterSenders { get; set; }
        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; }
    }
}
