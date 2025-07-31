using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CRMYonetimSistemi.Models; 

namespace CRMYonetimSistemi.Models
{
    public class Sale
    {
        public int Id { get; set; }

        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Display(Name = "Satış Tarihi")]
        [DataType(DataType.Date)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Toplam Tutar")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Para Birimi")]
        public CurrencyType Currency { get; set; }

        [Display(Name = "Döviz Kuru")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? ExchangeRate { get; set; }

        [Display(Name = "Toplam Tutar (TL)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmountInTRY { get; set; }

        [Display(Name = "Oluşturma Tarihi")]
        public DateTime CreatedAt { get; set; }

        public List<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}
