using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiClick.Models
{
    public partial class ImageCl
    {

        public int ImageId { get; set; }
        public int UserId { get; set; }
        public string path { get; set; }

    }
}
