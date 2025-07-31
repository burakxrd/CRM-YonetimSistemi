using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMYonetimSistemi.Data;
using CRMYonetimSistemi.Models;
using CRMYonetimSistemi.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        // GET: Sales
        public async Task<IActionResult> Index()
        {
            var sales = _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDate);

            return View(await sales.ToListAsync());
        }

        // GET: Sales/Details/5
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

        // GET: Sales/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CreateSaleViewModel
            {
                AvailableCustomers = await _context.Customers.OrderBy(c => c.Name).ToListAsync()
            };
            await RepopulateCreateSaleViewModel(viewModel);
            return View(viewModel);
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSaleViewModel viewModel)
        {
            if (viewModel.Items == null || !viewModel.Items.Any() || viewModel.Items.All(i => i.Quantity == 0))
            {
                ModelState.AddModelError("Items", "Satışa en az bir ürün eklemelisiniz.");
            }

            if (viewModel.Currency == CurrencyType.USD && (viewModel.ExchangeRate == null || viewModel.ExchangeRate <= 0))
            {
                ModelState.AddModelError("ExchangeRate", "Dolar seçildiğinde döviz kuru girmek zorunludur.");
            }

            if (ModelState.IsValid)
            {
                foreach (var item in viewModel.Items!.Where(i => i.Quantity > 0))
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product == null || product.Stock < item.Quantity)
                    {
                        ModelState.AddModelError("", $"'{product?.Name ?? "Bilinmeyen Ürün"}' için yeterli stok bulunmamaktadır.");
                        await RepopulateCreateSaleViewModel(viewModel);
                        return View(viewModel);
                    }
                }

                decimal totalAmount = viewModel.Items!.Sum(item => (decimal)item.Quantity * item.UnitPrice);
                decimal exchangeRate = (viewModel.Currency == CurrencyType.USD && viewModel.ExchangeRate.HasValue) ? viewModel.ExchangeRate.Value : 1;
                decimal totalAmountInTRY = totalAmount * exchangeRate;

                var sale = new Sale
                {
                    CustomerId = viewModel.CustomerId,
                    SaleDate = viewModel.SaleDate,
                    CreatedAt = DateTime.Now,
                    TotalAmount = totalAmount,
                    Currency = viewModel.Currency,
                    ExchangeRate = viewModel.ExchangeRate,
                    TotalAmountInTRY = totalAmountInTRY,
                    SaleItems = new List<SaleItem>()
                };

                foreach (var item in viewModel.Items!)
                {
                    sale.SaleItems.Add(new SaleItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock -= item.Quantity;
                    }
                }

                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await RepopulateCreateSaleViewModel(viewModel);
            return View(viewModel);
        }

        // GET: Sales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.SaleItems)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return NotFound();

            var viewModel = new EditSaleViewModel
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                SaleDate = sale.SaleDate,
                Currency = sale.Currency,
                ExchangeRate = sale.ExchangeRate,
                Items = sale.SaleItems.Select(si => new SaleItemViewModel
                {
                    ProductId = si.ProductId,
                    Quantity = si.Quantity,
                    UnitPrice = si.UnitPrice
                }).ToList(),
                AvailableProducts = await _context.Products.ToListAsync(),
                AvailableCustomers = await _context.Customers.ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditSaleViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (viewModel.Currency == CurrencyType.USD && (viewModel.ExchangeRate == null || viewModel.ExchangeRate <= 0))
            {
                ModelState.AddModelError("ExchangeRate", "Dolar seçildiğinde döviz kuru girmek zorunludur.");
            }

            if (ModelState.IsValid)
            {
                var saleInDb = await _context.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Product).FirstOrDefaultAsync(s => s.Id == id);
                if (saleInDb == null) return NotFound();

                foreach (var itemViewModel in viewModel.Items)
                {
                    var saleItemInDb = saleInDb.SaleItems.FirstOrDefault(si => si.ProductId == itemViewModel.ProductId);
                    if (saleItemInDb != null)
                    {
                        int quantityDifference = itemViewModel.Quantity - saleItemInDb.Quantity;
                        if (saleItemInDb.Product != null && saleItemInDb.Product.Stock < quantityDifference)
                        {
                            ModelState.AddModelError("", $"'{saleItemInDb.Product.Name}' ürünü için yeterli stok bulunmamaktadır.");
                            viewModel.AvailableProducts = await _context.Products.ToListAsync();
                            viewModel.AvailableCustomers = await _context.Customers.ToListAsync();
                            return View(viewModel);
                        }
                    }
                }

                saleInDb.CustomerId = viewModel.CustomerId;
                saleInDb.SaleDate = viewModel.SaleDate;
                saleInDb.Currency = viewModel.Currency;
                saleInDb.ExchangeRate = viewModel.ExchangeRate;

                foreach (var itemViewModel in viewModel.Items)
                {
                    var saleItemInDb = saleInDb.SaleItems.FirstOrDefault(si => si.ProductId == itemViewModel.ProductId);
                    if (saleItemInDb != null)
                    {
                        int quantityDifference = itemViewModel.Quantity - saleItemInDb.Quantity;
                        if (saleItemInDb.Product != null)
                        {
                            saleItemInDb.Product.Stock -= quantityDifference;
                        }
                        saleItemInDb.Quantity = itemViewModel.Quantity;
                        saleItemInDb.UnitPrice = itemViewModel.UnitPrice;
                    }
                }

                decimal totalAmount = saleInDb.SaleItems.Sum(i => (decimal)i.Quantity * i.UnitPrice);
                decimal exchangeRate = (saleInDb.Currency == CurrencyType.USD && saleInDb.ExchangeRate.HasValue) ? saleInDb.ExchangeRate.Value : 1;
                saleInDb.TotalAmount = totalAmount;
                saleInDb.TotalAmountInTRY = totalAmount * exchangeRate;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(viewModel.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.AvailableProducts = await _context.Products.ToListAsync();
            viewModel.AvailableCustomers = await _context.Customers.ToListAsync();
            return View(viewModel);
        }

        // GET: Sales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var sale = await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null) return NotFound();

            return View(sale);
        }

        // POST: Sales/Delete/5
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

        private async Task RepopulateCreateSaleViewModel(CreateSaleViewModel viewModel)
        {
            var products = await _context.Products.Where(p => p.Stock > 0).OrderBy(p => p.Name).ToListAsync();
            var allPurchaseHistories = await _context.ProductPurchaseHistories.ToListAsync();
            var availableProductsForSale = new List<ProductInfoForSaleViewModel>();

            foreach (var product in products)
            {
                var historiesForProduct = allPurchaseHistories.Where(h => h.ProductId == product.Id);
                decimal totalCost = historiesForProduct.Sum(h => h.TotalCostInTRY);
                int totalQuantity = historiesForProduct.Sum(h => h.Quantity);
                decimal averageCost = (totalQuantity > 0) ? totalCost / totalQuantity : 0;

                availableProductsForSale.Add(new ProductInfoForSaleViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Stock = product.Stock,
                    AverageUnitCostInTRY = averageCost
                });
            }

            viewModel.AvailableProducts = availableProductsForSale;
            viewModel.AvailableCustomers = await _context.Customers.OrderBy(c => c.Name).ToListAsync();
        }

        private bool SaleExists(int id)
        {
            return _context.Sales.Any(e => e.Id == id);
        }
    }
}
