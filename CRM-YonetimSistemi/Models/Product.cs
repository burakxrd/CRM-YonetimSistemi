using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Ürün Adı")]
        [Required]
        public string? Name { get; set; }

        [Display(Name = "Stok Adedi")]
        public int Stock { get; set; } 
    }
}