using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMYonetimSistemi.Models
{
    public enum CurrencyType
    {
        [Display(Name = "TL")]
        TRY = 0, // Veritabanında 0 olarak saklanır
        [Display(Name = "Dolar")]
        USD = 1  // Veritabanında 1 olarak saklanır
    }

    public class ProductPurchaseHistory
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        public DateTime PurchaseDate { get; set; }

        [Display(Name = "Miktar (Adet)")]
        public int Quantity { get; set; }

        [Display(Name = "Ürün Birim Fiyatı")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ProductPricePerUnit { get; set; }

        [Display(Name = "Toplam Ağırlık (Kg)")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal TotalKg { get; set; }

        [Display(Name = "Kargo Ücreti (Kg Başına)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingCostPerKg { get; set; }

        [Display(Name = "Toplam Ürün Fiyatı")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ProductPrice { get; set; }

        [Display(Name = "Toplam Kargo Maliyeti")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingCost { get; set; }

        [Display(Name = "Ürün Para Birimi")]
        public CurrencyType ProductCurrency { get; set; }

        [Display(Name = "Kargo Para Birimi")]
        public CurrencyType ShippingCurrency { get; set; }


        [Display(Name = "Döviz Kuru")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal? ExchangeRate { get; set; }

        [Display(Name = "Toplam Maliyet (TL)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCostInTRY { get; set; }

        public ProductPurchaseHistory()
        {
            PurchaseDate = DateTime.Now;
        }
    }
}