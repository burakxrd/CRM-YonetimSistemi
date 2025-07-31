using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CRMYonetimSistemi.Models;

namespace CRMYonetimSistemi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ProductPurchaseHistory> ProductPurchaseHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<ProductPurchaseHistory>(entity =>
            {
                entity.Property(p => p.ProductPrice).HasColumnType("decimal(18, 2)");
                entity.Property(p => p.ShippingCost).HasColumnType("decimal(18, 2)");
                entity.Property(p => p.ExchangeRate).HasColumnType("decimal(18, 4)");
                entity.Property(p => p.TotalCostInTRY).HasColumnType("decimal(18, 2)");

            });

            builder.Entity<SaleItem>(entity =>
            {
                entity.Property(si => si.UnitPrice).HasColumnType("decimal(18, 2)");
            });

            builder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18, 2)");
            builder.Entity<Expense>().Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            builder.Entity<Sale>().Property(s => s.TotalAmount).HasColumnType("decimal(18, 2)");
        }
    }
}
