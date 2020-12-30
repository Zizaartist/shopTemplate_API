using ApiClick.Models;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    /// <summary>
    /// Модель, лайков и дизлайков
    /// </summary>
    public partial class MessageOpinion
    {
        //Not nullable
        [Key]
        public int MessageOpinionId { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }
        /// <summary>
        /// При значении true - лайк, false - дизлайк
        /// </summary>
        public bool Opinion { get; set; }

        [ForeignKey("MessageId")]
        public Message Message { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

    }
}
