using CRMYonetimSistemi.Models;
using System.Collections.Generic;

namespace CRMYonetimSistemi.ViewModels
{
    public class CustomerProfileViewModel
    {
        // Müşterinin kendi bilgilerini tutar
        public Customer Customer { get; set; } = new Customer();

        // Müşteriye yapılan satışların listesini tutar
        public List<Sale> Sales { get; set; } = new List<Sale>();
    }
}
