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
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Product için hassasiyet ayarları
            builder.Entity<Product>().Property(p => p.ProductPriceInUSD).HasColumnType("decimal(18, 2)");
            builder.Entity<Product>().Property(p => p.Kg).HasColumnType("decimal(18, 4)");
            builder.Entity<Product>().Property(p => p.ShippingCostPerKg).HasColumnType("decimal(18, 2)");
            builder.Entity<Product>().Property(p => p.ExchangeRate).HasColumnType("decimal(18, 4)");
            builder.Entity<Product>().Property(p => p.DefaultSellingPrice).HasColumnType("decimal(18, 2)");
            builder.Entity<Product>().Property(p => p.CalculatedCost).HasColumnType("decimal(18, 2)");

            builder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(18, 2)");

            builder.Entity<Expense>().Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            builder.Entity<SaleItem>().Property(si => si.UnitPrice).HasColumnType("decimal(18, 2)");

            builder.Entity<Sale>().Property(s => s.TotalAmount).HasColumnType("decimal(18, 2)");
        }
    }
}