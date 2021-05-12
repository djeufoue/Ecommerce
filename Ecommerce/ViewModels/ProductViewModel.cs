using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.ViewModels
{
    public class ProductViewModel
    {
        public long Id { get; set; } // auto-incremented primary key

        public long UserId { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(30)]
        public String Description { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime publishedAt { get; set; }

        public string ImageURL { get; set; }

    }
}
