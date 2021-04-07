using ApiClick.Models.EnumModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
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

        public int UserId { get; set; }
        public string Phone { get; set; }
        public UserRole UserRole { get; set; }
        public decimal Points { get; set; }
        public bool NotificationsEnabled { get; set; }
        public string NotificationRegistration { get; set; }
        public string DeviceType { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual UserInfo UserInfo { get; set; }
        public Executor Executor { get; set; }
        public ICollection<ErrorReport> ErrorReports { get; set; }
        public ICollection<Order> Orders { get; set; }
        [JsonIgnore]
        public ICollection<PointRegister> PointRegisterReceivers { get; set; }
        [JsonIgnore]
        public ICollection<PointRegister> PointRegisterSenders { get; set; }
        [JsonIgnore]
        public ICollection<Review> Reviews { get; set; }
    }
}
