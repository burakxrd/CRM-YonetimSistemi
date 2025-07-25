using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Display(Name = "Müşteri Adı")]
        public string? Name { get; set; }

        [Display(Name = "İletişim Bilgisi")]
        public string? ContactInfo { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime CreatedAt { get; set; }

        // Bu, bir müşterinin birden çok satışı olabileceğini belirtir.
        // Bu satır, .Include(c => c.Sales) sorgusunun çalışmasını sağlar.
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
