using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMYonetimSistemi.Data;
using CRMYonetimSistemi.Models;
using CRMYonetimSistemi.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace CRMYonetimSistemi.Controllers
{
    [Authorize]
    public class SalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var sales = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                .OrderByDescending(s => s.SaleDate);
            return View(await sales.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            return View(sale);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateSaleViewModel
            {
                AvailableProducts = await _context.Products.Where(p => p.Stock > 0).ToListAsync(),
                AvailableCustomers = await _context.Customers.ToListAsync()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSaleViewModel viewModel)
        {
            if (viewModel.Items == null || !viewModel.Items.Any() || viewModel.Items.All(i => i.Quantity == 0))
            {
                ModelState.AddModelError("Items", "Satışa en az bir ürün eklemelisiniz.");
            }

            if (ModelState.IsValid)
            {
                var sale = new Sale
                {
                    CustomerId = viewModel.CustomerId,
                    SaleDate = viewModel.SaleDate,
                    CreatedAt = DateTime.Now,
                    SaleItems = new List<SaleItem>()
                };

                decimal totalAmount = 0;

                foreach (var item in viewModel.Items!.Where(i => i.Quantity > 0))
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        var saleItem = new SaleItem
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = product.EffectiveSellingPrice
                        };
                        sale.SaleItems.Add(saleItem);
                        totalAmount += saleItem.Quantity * saleItem.UnitPrice;
                        product.Stock -= item.Quantity;
                    }
                }

                sale.TotalAmount = totalAmount;
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableProducts = await _context.Products.Where(p => p.Stock > 0).ToListAsync();
            viewModel.AvailableCustomers = await _context.Customers.ToListAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            return View(sale);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Product).FirstOrDefaultAsync(s => s.Id == id);
            if (sale != null)
            {
                foreach (var item in sale.SaleItems)
                {
                    if (item.Product != null)
                    {
                        item.Product.Stock += item.Quantity;
                    }
                }

                _context.Sales.Remove(sale);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
