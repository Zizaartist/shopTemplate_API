using ApiClick.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Click.Models
{
    public class HashTagCl
    {
        public int HashTagId { get; set; }
        public string HashTagName { get; set; }
        public int CategoryId { get; set; }

        public virtual CategoryCl Category { get; set; } 

    }
}
