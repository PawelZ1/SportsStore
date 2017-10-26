using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web.Mvc;

namespace SportsStore.Domain.Entities
{
    public class Product
    {
        [HiddenInput(DisplayValue = false)]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Podaj nazwę produktu")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Podaj opis produktu")]
        [DataType(DataType.MultilineText), Display(Name = "Opis")]
        public string Description { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Podaj dodatnią cenę")]
        [Display(Name = "Cena")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Podaj kategorię produktu")]
        [Display(Name = "Kategoria")]
        public string Category { get; set; }
    }
}
