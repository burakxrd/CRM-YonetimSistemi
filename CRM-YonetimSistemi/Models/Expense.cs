using System.ComponentModel.DataAnnotations;

namespace CRMYonetimSistemi.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Tutar")]
        public decimal Amount { get; set; }

        [Display(Name = "Gider Tarihi")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime CreatedAt { get; set; }
    }
}
