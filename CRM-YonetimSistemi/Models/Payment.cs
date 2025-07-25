using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRMYonetimSistemi.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [Display(Name = "Tutar")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; } // '?' opsiyonel (nullable) olmasını sağlar.

        [Display(Name = "Ödeme Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
