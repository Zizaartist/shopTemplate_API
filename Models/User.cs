using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using ApiClick.Models.EnumModels;
using ApiClick.StaticValues;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, содержащая пользовательские данные
    /// </summary>
    [Serializable]
    public partial class User
    {
        public User() 
        {
            Brands = new List<Brand>();
            Messages = new List<Message>();
            Orders = new List<Order>();
        }

        //Not nullable
        [Key]
        public int UserId { get; set; } 
        [Required, MaxLength(ModelLengths.LENGTH_SMALL)]
        public string Phone { get; set; }
        public UserRole UserRole { get; set; }
        public decimal Points { get; set; }
        public bool NotificationsEnabled { get; set; }

        //Nullable
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string NotificationRegistration { get; set; }
        [MaxLength(ModelLengths.LENGTH_MIN)]
        public string DeviceType { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Login { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Password { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Name { get; set; }
        [MaxLength(ModelLengths.LENGTH_MEDIUM)]
        public string Street { get; set; }
        [MaxLength(ModelLengths.LENGTH_MIN)]
        public string House { get; set; }
        public int? Padik { get; set; }
        public int? Etash { get; set; }
        public int? Kv { get; set; }
        public DateTime CreatedDate { get; set; }

        //Теневые свойства
        [NotMapped]
        public virtual ICollection<Brand> Brands { get; set; }
        [NotMapped]
        public virtual ICollection<Message> Messages { get; set; }
        [NotMapped]
        public virtual ICollection<Order> Orders { get; set; }
        [NotMapped]
        public ICollection<Image> UploadedImages { get; set; }
    }
}
