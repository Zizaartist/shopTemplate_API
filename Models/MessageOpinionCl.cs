using ApiClick.Models;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, лайков и дизлайков
    /// </summary>
    public partial class MessageOpinionCl
    {
        //Not nullable
        public int MessageOpinionId { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        /// <summary>
        /// При значении true - лайк, false - дизлайк
        /// </summary>
        public bool Opinion { get; set; } 

        public MessageCl Message { get; set; }
        public UserCl User { get; set; }

    }
}
