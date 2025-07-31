using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMYonetimSistemi.Models
{
    public class SaleItem
    {
        public int Id { get; set; }

        public int SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Display(Name = "Miktar (Adet)")]
        public int Quantity { get; set; }


        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }
    }
}
