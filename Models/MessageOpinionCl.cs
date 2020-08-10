using ApiClick.Models;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    public partial class MessageOpinionCl
    {

        public int MessageOpinionId { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        /// <summary>
        /// При значении true - лайк, при обратном - дизлайк
        /// </summary>
        public bool opinion { get; set; } 

        public MessageCl message { get; set; }
        public UserCl user { get; set; }

    }
}
