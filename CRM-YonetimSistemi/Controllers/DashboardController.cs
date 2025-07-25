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
            var sales = await _context.Sales
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .Include(s => s.Customer)
                .ToListAsync();

            var expenses = await _context.Expenses.ToListAsync();
            var customers = await _context.Customers.ToListAsync();
            var payments = await _context.Payments.ToListAsync();

            decimal totalRevenue = sales.Sum(s => s.TotalAmount);
            decimal totalExpenses = expenses.Sum(e => e.Amount);
            int totalCustomers = customers.Count();

            decimal costOfGoodsSold = sales.SelectMany(s => s.SaleItems)
                                            .Where(si => si.Product != null)
                                            .Sum(si => si.Product!.CalculatedCost * si.Quantity);

            decimal netProfit = totalRevenue - totalExpenses - costOfGoodsSold;
            decimal currentBalance = totalRevenue - totalExpenses;

            var customerSales = sales.GroupBy(s => s.CustomerId)
                                     .ToDictionary(g => g.Key, g => g.Sum(s => s.TotalAmount));
            var customerPayments = payments.GroupBy(p => p.CustomerId)
                                           .ToDictionary(g => g.Key, g => g.Sum(p => p.Amount));

            decimal remainingReceivables = 0;
            foreach (var customer in customers)
            {
                customerSales.TryGetValue(customer.Id, out decimal totalSale);
                customerPayments.TryGetValue(customer.Id, out decimal totalPayment);
                decimal balance = totalSale - totalPayment;
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
                    TotalAmount = g.Sum(s => s.TotalAmount)
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

