using Microsoft.AspNetCore.Mvc.Rendering;
using CRMYonetimSistemi.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.ViewModels
{
    public class CreateSaleViewModel
    {
        [Required(ErrorMessage = "Müşteri seçimi zorunludur.")]
        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Satış tarihi zorunludur.")]
        [Display(Name = "Satış Tarihi")]
        public DateTime SaleDate { get; set; } = DateTime.Today;

        // Formdan gelen ürün listesi
        public List<SaleItemViewModel> Items { get; set; } = new List<SaleItemViewModel>();

        // Dropdown listeleri ve diğer verileri View'a taşımak için
        public IEnumerable<Product> AvailableProducts { get; set; } = new List<Product>();
        public IEnumerable<Customer> AvailableCustomers { get; set; } = new List<Customer>();
    }

    // Satış formundaki her bir ürün satırını temsil eder
    public class SaleItemViewModel
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Adet en az 1 olmalıdır.")]
        public int Quantity { get; set; }
    }
}
