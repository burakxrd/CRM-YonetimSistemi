using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRMYonetimSistemi.Data;
using CRMYonetimSistemi.Models;
using Microsoft.AspNetCore.Authorization;

namespace CRMYonetimSistemi.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ProductPriceInUSD,Kg,ShippingCostPerKg,ExchangeRate,DefaultSellingPrice,Stock,CreatedAt")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.CalculatedCost = product.Kg * product.ShippingCostPerKg;
                product.CreatedAt = DateTime.Now;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProductPriceInUSD,Kg,ShippingCostPerKg,ExchangeRate,DefaultSellingPrice,Stock")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // ÖNEMLİ: Orijinal veriyi veritabanından çekip sadece kullanıcıdan gelen alanları güncelliyoruz.
                    var productInDb = await _context.Products.FindAsync(id);
                    if (productInDb == null)
                    {
                        return NotFound();
                    }

                    // Formdan gelen değerleri ata
                    productInDb.Name = product.Name;
                    productInDb.ProductPriceInUSD = product.ProductPriceInUSD;
                    productInDb.Kg = product.Kg;
                    productInDb.ShippingCostPerKg = product.ShippingCostPerKg;
                    productInDb.ExchangeRate = product.ExchangeRate;
                    productInDb.DefaultSellingPrice = product.DefaultSellingPrice;
                    productInDb.Stock = product.Stock;

                    // YENİ: Maliyet otomatik olarak yeniden hesaplanıyor.
                    productInDb.CalculatedCost = product.Kg * product.ShippingCostPerKg;

                    _context.Update(productInDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
