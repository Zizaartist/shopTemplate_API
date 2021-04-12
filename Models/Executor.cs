using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ApiClick.Models
{
    //Никогда не должна попасть в руки юзера
    public partial class Executor
    {
        public int ExecutorId { get; set; }
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public virtual Brand Brand { get; set; }
        public User User { get; set; }
    }
}
