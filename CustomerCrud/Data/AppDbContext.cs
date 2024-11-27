using CustomerCrud.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerCrud.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options) : base(options) { }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<Customers> Customers { get; set; }

        public string GetNextCustomerNo()
        {
            int nextId = Customers.Max(c => (int?)c.CustomersId) ?? 0;
            return (nextId + 1).ToString("D3");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedData.Seed(modelBuilder);
        }
    }
}
