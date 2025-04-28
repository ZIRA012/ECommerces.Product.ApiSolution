using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;

namespace ProductApi.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
        // Constructor que toma DbContextOptions
       
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        // DbSet para la entidad Product
        public DbSet<Product> Products { get; set; }
    }
}
