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
            Brand = new HashSet<Brand>();
            ErrorReport = new HashSet<ErrorReport>();
            Order = new HashSet<Order>();
            PointRegisterReceiver = new HashSet<PointRegister>();
            PointRegisterSender = new HashSet<PointRegister>();
            Review = new HashSet<Review>();
        }

        public int UserId { get; set; }
        public string Phone { get; set; }
        public int UserRole { get; set; }
        public decimal Points { get; set; }
        public bool? NotificationsEnabled { get; set; }
        public string NotificationRegistration { get; set; }
        public string DeviceType { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Executor Executor { get; set; }
        public virtual UserInfo UserInfo { get; set; }
        public virtual ICollection<Brand> Brand { get; set; }
        public virtual ICollection<ErrorReport> ErrorReport { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<PointRegister> PointRegisterReceiver { get; set; }
        public virtual ICollection<PointRegister> PointRegisterSender { get; set; }
        public virtual ICollection<Review> Review { get; set; }
    }
}
