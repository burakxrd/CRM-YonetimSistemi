namespace CRMYonetimSistemi.Models
{
    public class CustomerProductPrice
    {
        public int Id { get; set; } // Birincil Anahtar
        public int ProductId { get; set; } // Foreign Key
        public int CustomerId { get; set; } // Foreign Key
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public virtual Product? Product { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}