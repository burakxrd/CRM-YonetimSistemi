using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using CRMYonetimSistemi.Data;
using CRMYonetimSistemi.Documents;

namespace CRMYonetimSistemi.Controllers
{
    [Authorize]
    public class ProformaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProformaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Generate(int id)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.SaleItems) // Çok önemli! Satış kalemlerini de getiriyoruz.
                    .ThenInclude(si => si.Product) // Kalemlerin içindeki ürünleri de getiriyoruz.
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            var document = new ProformaDocument(sale);
            byte[] pdfBytes = document.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Proforma-{sale.Id}-{sale.Customer!.Name}.pdf");
        }
    }
}
