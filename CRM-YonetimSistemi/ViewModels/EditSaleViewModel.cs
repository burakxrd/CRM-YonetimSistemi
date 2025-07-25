using Microsoft.AspNetCore.Mvc.Rendering;
using CRMYonetimSistemi.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.ViewModels
{
    // Bu yeni ViewModel, hem mevcut satış bilgilerini hem de
    // formdaki dinamik ürün listesini tutar.
    public class EditSaleViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Müşteri seçimi zorunludur.")]
        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Satış tarihi zorunludur.")]
        [Display(Name = "Satış Tarihi")]
        public DateTime SaleDate { get; set; }

        // Formdan gelen ürün listesi
        public List<SaleItemViewModel> Items { get; set; } = new List<SaleItemViewModel>();

        // Dropdown listeleri için
        public IEnumerable<Product> AvailableProducts { get; set; } = new List<Product>();
        public IEnumerable<Customer> AvailableCustomers { get; set; } = new List<Customer>();
    }
}
