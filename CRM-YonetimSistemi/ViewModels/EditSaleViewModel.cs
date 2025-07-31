using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CRMYonetimSistemi.Models;

namespace CRMYonetimSistemi.ViewModels
{
    public class EditSaleViewModel
    {
        public int Id { get; set; }

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

        public List<SaleItemViewModel> Items { get; set; }

        public IEnumerable<Product> AvailableProducts { get; set; }
        public IEnumerable<Customer> AvailableCustomers { get; set; }

        public EditSaleViewModel()
        {
            Items = new List<SaleItemViewModel>();
            AvailableProducts = new List<Product>();
            AvailableCustomers = new List<Customer>();
        }
    }
}
