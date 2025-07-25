using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMYonetimSistemi.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "Ürün Adı")]
        public string? Name { get; set; }

        [Display(Name = "Ürün Fiyatı (USD)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ProductPriceInUSD { get; set; }

        [Display(Name = "Kilogram (Kg)")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Kg { get; set; }

        [Display(Name = "Kargo Ücreti (Kg Başına ₺)")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingCostPerKg { get; set; }

        [Display(Name = "Döviz Kuru")]
        [Column(TypeName = "decimal(18, 4)")]
        public decimal ExchangeRate { get; set; }

        [Display(Name = "Varsayılan Satış Fiyatı")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? DefaultSellingPrice { get; set; }

        [Display(Name = "Stok Adedi")]
        public int Stock { get; set; }

        [Display(Name = "Hesaplanan Maliyet")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal CalculatedCost { get; set; }

        [Display(Name = "Oluşturma Tarihi")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        // Bu özellik veritabanında bir sütun oluşturmaz.
        // Sadece kod içinde kullanılırken dinamik olarak bir değer hesaplar.
        [NotMapped]
        public decimal EffectiveSellingPrice
        {
            get
            {
                // Eğer bir Varsayılan Satış Fiyatı girilmişse, onu kullan.
                // Girilmemişse, USD Fiyat * Kur formülüyle hesapla.
                return DefaultSellingPrice ?? (ProductPriceInUSD * ExchangeRate);
            }
        }
    }
}
