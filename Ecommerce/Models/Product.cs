using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Models
{
    public class Product
    {
        //public string Id { get; set; } // user-provided primary key
        public long Id { get; set; } // auto-incremented primary key

        public long UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        [MaxLength(30)]
        public string Title { get; set; }

        [MaxLength(4095)]
        public String  Description { get; set; }
    
        public int Price { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime publishedAt { get; set; }

        public string ImageURL { get; set; }
    }
}
