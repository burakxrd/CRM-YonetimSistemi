using System.Collections.Generic;
using CRMYonetimSistemi.Models;

namespace CRMYonetimSistemi.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int TotalCustomers { get; set; }
        public decimal RemainingReceivables { get; set; }
        public decimal CurrentBalance { get; set; }

        // Listeleri boş olarak başlatarak null olma ihtimalini ortadan kaldırıyoruz.
        public List<Product> LowStockProducts { get; set; } = new List<Product>();
        public List<TopCustomerViewModel> TopCustomers { get; set; } = new List<TopCustomerViewModel>();
    }

    public class TopCustomerViewModel
    {
        // Bu özelliği de null olamayacak şekilde başlatıyoruz.
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
