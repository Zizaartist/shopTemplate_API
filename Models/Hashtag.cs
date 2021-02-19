using ApiClick.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ApiClick.StaticValues;

namespace Click.Models
{
    public class Hashtag
    {
        [Key]
        public int HashTagId { get; set; }
        public string HashTagName { get; set; }
        public Category Category { get; set; }
    }
}
