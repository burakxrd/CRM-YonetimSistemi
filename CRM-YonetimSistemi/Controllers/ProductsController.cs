using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRMYonetimSistemi.Data;
using CRMYonetimSistemi.Models;
using CRMYonetimSistemi.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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

        // GET: Products - Ürünleri ve ortalama birim maliyetlerini listeler
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.OrderBy(p => p.Name).ToListAsync();
            var allPurchaseHistories = await _context.ProductPurchaseHistories.ToListAsync();
            var viewModelList = new List<ProductIndexViewModel>();

            foreach (var product in products)
            {
                var historiesForProduct = allPurchaseHistories.Where(h => h.ProductId == product.Id);
                decimal totalCost = historiesForProduct.Sum(h => h.TotalCostInTRY);
                int totalQuantity = historiesForProduct.Sum(h => h.Quantity);
                decimal averageCost = (totalQuantity > 0) ? totalCost / totalQuantity : 0;

                viewModelList.Add(new ProductIndexViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Stock = product.Stock,
                    AverageUnitCostInTRY = averageCost
                });
            }
            return View(viewModelList);
        }

        #region Ürün Tanımlama (Create/Edit)

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.Stock = 0;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Product product)
        {
            if (id != product.Id) return NotFound();
            var productInDb = await _context.Products.FindAsync(id);
            if (productInDb == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    productInDb.Name = product.Name;
                    _context.Update(productInDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        #endregion

        #region Ürün Alımı (Purchase/EditPurchase)

        // GET: Products/Purchase
        public async Task<IActionResult> Purchase()
        {
            var viewModel = new ProductPurchaseViewModel
            {
                Products = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name"),
                ProductCurrency = CurrencyType.USD, // Varsayılan değer
                ShippingCurrency = CurrencyType.USD // Varsayılan değer
            };
            return View(viewModel);
        }

        // POST: Products/Purchase
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(ProductPurchaseViewModel model)
        {
            // Döviz kuru validasyonu: Ürün veya Kargo para birimi Dolar ise ve kur girilmemişse veya 0'dan küçükse hata ver.
            if ((model.ProductCurrency == CurrencyType.USD || model.ShippingCurrency == CurrencyType.USD) && (model.ExchangeRate == null || model.ExchangeRate <= 0))
            {
                ModelState.AddModelError("ExchangeRate", "Dolar seçilen bir para birimi olduğunda döviz kuru girmek zorunludur ve pozitif olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null) return NotFound();

                decimal exchangeRate = model.ExchangeRate ?? 1; // Eğer boşsa veya invalidse 1 olarak kabul et, validasyon hatası vermediyse

                // Hesaplamalar
                decimal productPriceTotal = model.ProductPricePerUnit * model.QuantityInUnits;
                decimal shippingCostTotal = model.TotalKg * model.ShippingCostPerKg;

                decimal productCostInTRY = (model.ProductCurrency == CurrencyType.USD) ? productPriceTotal * exchangeRate : productPriceTotal;
                decimal shippingCostInTRY = (model.ShippingCurrency == CurrencyType.USD) ? shippingCostTotal * exchangeRate : shippingCostTotal;

                decimal totalCostInTRY = productCostInTRY + shippingCostInTRY;

                var purchaseHistory = new ProductPurchaseHistory
                {
                    ProductId = model.ProductId,
                    PurchaseDate = DateTime.Now,
                    Quantity = model.QuantityInUnits,
                    ProductPricePerUnit = model.ProductPricePerUnit,
                    TotalKg = model.TotalKg,
                    ShippingCostPerKg = model.ShippingCostPerKg,
                    ProductPrice = productPriceTotal,
                    ShippingCost = shippingCostTotal,
                    ProductCurrency = model.ProductCurrency,
                    ShippingCurrency = model.ShippingCurrency,
                    ExchangeRate = model.ExchangeRate,
                    TotalCostInTRY = totalCostInTRY
                };

                // Ürün alımını gider olarak kaydet
                var expense = new Expense
                {
                    Description = $"Ürün satın alındı: {product.Name} (Alım ID: {purchaseHistory.Id})", // Alım ID'sini ekleyerek benzersizlik sağlamaya çalış
                    Amount = totalCostInTRY, // TL cinsinden toplam maliyeti gider olarak ekle
                    Date = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                product.Stock += model.QuantityInUnits;
                _context.Add(purchaseHistory);
                _context.Add(expense); // Gideri ekle
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(PurchaseHistory));
            }

            model.Products = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", model.ProductId);
            return View(model);
        }

        // GET: Products/EditPurchase/5
        public async Task<IActionResult> EditPurchase(int id)
        {
            var purchaseHistory = await _context.ProductPurchaseHistories.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == id);
            if (purchaseHistory == null) return NotFound();

            var viewModel = new ProductPurchaseViewModel
            {
                Id = purchaseHistory.Id,
                ProductId = purchaseHistory.ProductId,
                QuantityInUnits = purchaseHistory.Quantity,
                ProductPricePerUnit = purchaseHistory.ProductPricePerUnit,
                TotalKg = purchaseHistory.TotalKg,
                ShippingCostPerKg = purchaseHistory.ShippingCostPerKg,
                ProductCurrency = purchaseHistory.ProductCurrency,
                ShippingCurrency = purchaseHistory.ShippingCurrency,
                ExchangeRate = purchaseHistory.ExchangeRate,
                Products = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", purchaseHistory.ProductId)
            };
            return View("Purchase", viewModel);
        }

        // POST: Products/EditPurchase/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPurchase(int id, ProductPurchaseViewModel model)
        {
            if (id != model.Id) return NotFound();

            // Döviz kuru validasyonu: Ürün veya Kargo para birimi Dolar ise ve kur girilmemişse veya 0'dan küçükse hata ver.
            if ((model.ProductCurrency == CurrencyType.USD || model.ShippingCurrency == CurrencyType.USD) && (model.ExchangeRate == null || model.ExchangeRate <= 0))
            {
                ModelState.AddModelError("ExchangeRate", "Dolar seçilen bir para birimi olduğunda döviz kuru girmek zorunludur ve pozitif olmalıdır.");
            }

            if (ModelState.IsValid)
            {
                var purchaseInDb = await _context.ProductPurchaseHistories.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == id);
                var product = await _context.Products.FindAsync(model.ProductId); // Yeni seçilen ürün
                if (purchaseInDb == null || product == null) return NotFound();

                // Eski üründen stoğu düş
                var oldProduct = await _context.Products.FindAsync(purchaseInDb.ProductId);
                if (oldProduct != null)
                {
                    oldProduct.Stock -= purchaseInDb.Quantity;
                    _context.Update(oldProduct);
                }

                // Döviz kurunu varsayılan olarak 1 al
                decimal exchangeRate = model.ExchangeRate ?? 1;

                // Hesaplamalar
                decimal productPriceTotal = model.ProductPricePerUnit * model.QuantityInUnits;
                decimal shippingCostTotal = model.TotalKg * model.ShippingCostPerKg;

                decimal productCostInTRY = (model.ProductCurrency == CurrencyType.USD) ? productPriceTotal * exchangeRate : productPriceTotal;
                decimal shippingCostInTRY = (model.ShippingCurrency == CurrencyType.USD) ? shippingCostTotal * exchangeRate : shippingCostTotal;

                decimal totalCostInTRY = productCostInTRY + shippingCostInTRY;

                // Eski gider kaydını bul ve sil (açıklama ve ID'ye göre)
                // Dikkat: Bu yöntem, açıklama formatı değişirse eski kaydı bulamayabilir.
                // Daha sağlam bir çözüm için Expense modeline ProductPurchaseHistoryId gibi bir FK eklenebilir.
                var oldExpenseDescription = $"Ürün satın alındı: {purchaseInDb.Product?.Name} (Alım ID: {purchaseInDb.Id})";
                var existingExpense = await _context.Expenses.FirstOrDefaultAsync(e => e.Description == oldExpenseDescription);

                if (existingExpense != null)
                {
                    _context.Expenses.Remove(existingExpense);
                }

                // Yeni gider kaydını oluştur
                var newExpense = new Expense
                {
                    Description = $"Ürün satın alındı: {product.Name} (Alım ID: {id})", // Yeni ürün adı ve mevcut alım ID'si ile
                    Amount = totalCostInTRY,
                    Date = DateTime.Now,
                    CreatedAt = DateTime.Now
                };
                _context.Add(newExpense);


                // ProductPurchaseHistory kaydını güncelle
                purchaseInDb.ProductId = model.ProductId; // Eğer ürün değiştiyse
                purchaseInDb.PurchaseDate = DateTime.Now;
                purchaseInDb.Quantity = model.QuantityInUnits;
                purchaseInDb.ProductPricePerUnit = model.ProductPricePerUnit;
                purchaseInDb.TotalKg = model.TotalKg;
                purchaseInDb.ShippingCostPerKg = model.ShippingCostPerKg;
                purchaseInDb.ProductPrice = productPriceTotal;
                purchaseInDb.ShippingCost = shippingCostTotal;
                purchaseInDb.ProductCurrency = model.ProductCurrency;
                purchaseInDb.ShippingCurrency = model.ShippingCurrency;
                purchaseInDb.ExchangeRate = model.ExchangeRate;
                purchaseInDb.TotalCostInTRY = totalCostInTRY;

                // Yeni ürüne stoğu ekle
                product.Stock += model.QuantityInUnits;

                _context.Update(purchaseInDb);
                _context.Update(product); // Yeni ürünü güncelle
                // Eğer eski ürün farklıysa onu da kaydet
                if (oldProduct != null && oldProduct.Id != product.Id)
                {
                    _context.Update(oldProduct);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(PurchaseHistory));
            }

            model.Products = new SelectList(await _context.Products.OrderBy(p => p.Name).ToListAsync(), "Id", "Name", model.ProductId);
            return View("Purchase", model);
        }

        #endregion

        #region Alım Geçmişi Yönetimi (History/Delete)

        // GET: Products/PurchaseHistory
        public async Task<IActionResult> PurchaseHistory()
        {
            var history = await _context.ProductPurchaseHistories
                .Include(h => h.Product)
                .OrderByDescending(h => h.PurchaseDate)
                .ToListAsync();
            return View(history);
        }

        // POST: Products/DeletePurchase/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePurchase(int id, bool adjustStock)
        {
            var purchaseToDelete = await _context.ProductPurchaseHistories.Include(p => p.Product).FirstOrDefaultAsync(p => p.Id == id);
            if (purchaseToDelete == null) return NotFound();

            // İlgili gider kaydını sil
            var expenseDescription = $"Ürün satın alındı: {purchaseToDelete.Product?.Name} (Alım ID: {purchaseToDelete.Id})";
            var existingExpense = await _context.Expenses.FirstOrDefaultAsync(e => e.Description == expenseDescription);
            if (existingExpense != null)
            {
                _context.Expenses.Remove(existingExpense);
            }

            if (adjustStock)
            {
                var product = await _context.Products.FindAsync(purchaseToDelete.ProductId);
                if (product != null)
                {
                    product.Stock -= purchaseToDelete.Quantity;
                    _context.Update(product);
                }
            }
            _context.ProductPurchaseHistories.Remove(purchaseToDelete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PurchaseHistory));
        }

        // POST: Products/ClearPurchaseHistory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearPurchaseHistory(bool adjustStock)
        {
            if (adjustStock)
            {
                await _context.Products.ForEachAsync(p => p.Stock = 0);
            }

            // Tüm ürün alım geçmişini silerken, ilgili giderleri de temizle
            var allPurchaseHistories = await _context.ProductPurchaseHistories.Include(p => p.Product).ToListAsync();
            foreach (var history in allPurchaseHistories)
            {
                var expenseDescription = $"Ürün satın alındı: {history.Product?.Name} (Alım ID: {history.Id})";
                var existingExpense = await _context.Expenses.FirstOrDefaultAsync(e => e.Description == expenseDescription);
                if (existingExpense != null)
                {
                    _context.Expenses.Remove(existingExpense);
                }
            }

            _context.ProductPurchaseHistories.RemoveRange(allPurchaseHistories);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(PurchaseHistory));
        }

        #endregion

        #region Ürün Silme

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            bool hasPurchaseHistory = await _context.ProductPurchaseHistories.AnyAsync(h => h.ProductId == id);
            bool hasSaleHistory = await _context.SaleItems.AnyAsync(si => si.ProductId == id);

            if (hasPurchaseHistory || hasSaleHistory)
            {
                ViewBag.ErrorMessage = "Bu ürünün alım veya satış geçmişi bulunduğu için silemezsiniz.";
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasPurchaseHistory = await _context.ProductPurchaseHistories.AnyAsync(h => h.ProductId == id);
            bool hasSaleHistory = await _context.SaleItems.AnyAsync(si => si.ProductId == id);

            if (hasPurchaseHistory || hasSaleHistory)
            {
                TempData["ErrorMessage"] = "Bu ürünün alım veya satış geçmişi bulunduğu için silinemedi.";
                return RedirectToAction(nameof(Index));
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        #endregion

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}