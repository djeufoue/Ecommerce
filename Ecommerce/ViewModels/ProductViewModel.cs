using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
    public class ProductViewModel
    {
        public long Id { get; set; } // auto-incremented primary key

        [HiddenInput(DisplayValue = false)]
        public long UserId { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(5000)]
        public String Description { get; set; }

        public int Price { get; set; }

        public int Quantity { get; set; }

        [Display(Name =" Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Published At")]
        public DateTime publishedAt { get; set; }

        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; }

        [Display(Name ="Image")]
        public string ImageURL { get; set; }

        public int Phone { get; set; }

    }
}
