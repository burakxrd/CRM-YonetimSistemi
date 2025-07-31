using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CRMYonetimSistemi.Data;
using CRMYonetimSistemi.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace CRMYonetimSistemi.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales.Include(s => s.Customer).ToListAsync();
            var expenses = await _context.Expenses.ToListAsync();
            var customers = await _context.Customers.ToListAsync();
            var payments = await _context.Payments.ToListAsync();
            var purchaseHistories = await _context.ProductPurchaseHistories.ToListAsync();


            decimal totalRevenue = sales.Sum(s => s.TotalAmountInTRY);

            decimal totalExpenses = expenses.Sum(e => e.Amount);
            int totalCustomers = customers.Count();

            decimal netProfit = totalRevenue - totalExpenses;

            decimal currentBalance = totalRevenue - totalExpenses;

            var customerSales = sales.GroupBy(s => s.CustomerId)
                                     .ToDictionary(g => g.Key, g => g.Sum(s => s.TotalAmountInTRY));

            var customerPayments = payments.GroupBy(p => p.CustomerId)
                                           .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            decimal remainingReceivables = 0;
            foreach (var customer in customers)
            {
                customerSales.TryGetValue(customer.Id, out decimal totalSaleInTRY);
                customerPayments.TryGetValue(customer.Id, out decimal totalPayment);
                decimal balance = totalSaleInTRY - totalPayment;
                if (balance > 0)
                {
                    remainingReceivables += balance;
                }
            }

            var topCustomers = sales
                .Where(s => s.Customer != null)
                .GroupBy(s => s.Customer)
                .Select(g => new TopCustomerViewModel
                {
                    CustomerName = g.Key!.Name ?? "İsimsiz Müşteri",
                    TotalAmount = g.Sum(s => s.TotalAmountInTRY)
                })
                .OrderByDescending(c => c.TotalAmount)
                .Take(5)
                .ToList();

            var lowStockProducts = await _context.Products
                .Where(p => p.Stock < 10)
                .OrderBy(p => p.Stock)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                TotalCustomers = totalCustomers,
                CurrentBalance = currentBalance,
                RemainingReceivables = remainingReceivables,
                TopCustomers = topCustomers,
                LowStockProducts = lowStockProducts
            };

            return View(viewModel);
        }
    }
}