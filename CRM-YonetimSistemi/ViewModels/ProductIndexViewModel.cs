using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.ViewModels
{
    public class ProductIndexViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Ürün Adı")]
        public string? Name { get; set; }

        [Display(Name = "Birim Maliyet (₺)")]
        public decimal AverageUnitCostInTRY { get; set; }

        [Display(Name = "Stok Adedi")]
        public int Stock { get; set; }
    }
}