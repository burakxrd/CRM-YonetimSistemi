using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRMYonetimSistemi.Models; 

namespace CRMYonetimSistemi.ViewModels
{
    public class ProductPurchaseViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Satın Alınacak Ürün")]
        [Required(ErrorMessage = "Lütfen bir ürün seçin.")]
        public int ProductId { get; set; }
        public IEnumerable<SelectListItem> Products { get; set; }

        [Display(Name = "Miktar (Adet)")]
        [Required(ErrorMessage = "Miktar girilmesi zorunludur."), Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den büyük olmalıdır.")]
        public int QuantityInUnits { get; set; }

        [Display(Name = "Ürün Birim Fiyatı")]
        [Required(ErrorMessage = "Ürün birim fiyatı girilmesi zorunludur."), Range(0.01, double.MaxValue, ErrorMessage = "Ürün birim fiyatı 0'dan büyük olmalıdır.")]
        public decimal ProductPricePerUnit { get; set; }

        [Display(Name = "Toplam Kargo Ağırlığı (Kg)")]
        [Required(ErrorMessage = "Toplam ağırlık girilmesi zorunludur."), Range(0.01, double.MaxValue, ErrorMessage = "Toplam ağırlık 0'dan büyük olmalıdır.")]
        public decimal TotalKg { get; set; }

        [Display(Name = "Kargo Ücreti (Kg Başına)")]
        [Required(ErrorMessage = "Kg başına kargo ücreti girilmesi zorunludur."), Range(0, double.MaxValue, ErrorMessage = "Kargo ücreti negatif olamaz.")]
        public decimal ShippingCostPerKg { get; set; }

        [Display(Name = "Ürün Para Birimi")]
        public CurrencyType ProductCurrency { get; set; }

        [Display(Name = "Kargo Para Birimi")]
        public CurrencyType ShippingCurrency { get; set; }

        [Display(Name = "Döviz Kuru (USD/TRY)")]
        [Range(0.0001, double.MaxValue, ErrorMessage = "Döviz kuru 0'dan büyük olmalıdır.")]
        public decimal? ExchangeRate { get; set; }

        public ProductPurchaseViewModel()
        {
            ProductCurrency = CurrencyType.USD;
            ShippingCurrency = CurrencyType.USD;
            Products = new List<SelectListItem>();
        }
    }
}