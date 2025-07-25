using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.Models
{
    public class SaleItem
    {
        public int Id { get; set; }

        public int SaleId { get; set; }
        public Sale? Sale { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [Display(Name = "Adet")]
        public int Quantity { get; set; }

        [Display(Name = "Birim Fiyat (₺)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
    }
}
