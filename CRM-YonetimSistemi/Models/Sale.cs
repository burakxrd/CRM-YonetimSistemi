using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMYonetimSistemi.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [Display(Name = "Satış Tarihi")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Toplam Tutar (₺)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        // --- İLİŞKİLER ---

        [Display(Name = "Müşteri")] 
        public Customer Customer { get; set; } = null!;

        public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
