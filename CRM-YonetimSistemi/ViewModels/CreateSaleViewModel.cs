using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRMYonetimSistemi.Models;

namespace CRMYonetimSistemi.ViewModels
{
    // YARDIMCI SINIF 1: Satış ekranındaki ürün dropdown'ı için gerekli bilgileri tutar.
    // Bu sınıf, ürünün anlık hesaplanan maliyetini de taşıyabilmek için gereklidir.
    public class ProductInfoForSaleViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Stock { get; set; }
        public decimal AverageUnitCostInTRY { get; set; }
    }

    // ANA SINIF: Sayfanın genel modelini temsil eder.
    public class CreateSaleViewModel
    {
        [Display(Name = "Müşteri")]
        [Required(ErrorMessage = "Lütfen bir müşteri seçin.")]
        public int CustomerId { get; set; }

        [Display(Name = "Satış Tarihi")]
        [DataType(DataType.Date)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Para Birimi")]
        public CurrencyType Currency { get; set; }

        [Display(Name = "Döviz Kuru (USD/TRY)")]
        public decimal? ExchangeRate { get; set; }

        // Satılan ürünlerin listesini tutar. Her bir elemanı aşağıdaki SaleItemViewModel'dir.
        public List<SaleItemViewModel> Items { get; set; }

        // Dropdown'ları doldurmak için bu listeler kullanılır.
        public IEnumerable<ProductInfoForSaleViewModel> AvailableProducts { get; set; }
        public IEnumerable<Customer> AvailableCustomers { get; set; }

        public CreateSaleViewModel()
        {
            SaleDate = DateTime.Now;
            Currency = CurrencyType.TRY; // Varsayılan TL
            Items = new List<SaleItemViewModel>();
            AvailableProducts = new List<ProductInfoForSaleViewModel>();
            AvailableCustomers = new List<Customer>();
        }
    }

    // YARDIMCI SINIF 2: Her bir satış satırının modelini temsil eder.
    public class SaleItemViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Display(Name = "Miktar (Adet)")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Miktar en az 1 olmalıdır.")]
        public int Quantity { get; set; }

        [Display(Name = "Birim Fiyat")]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }
    }
}
