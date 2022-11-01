using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using UnicornSupplies;

namespace UnicornSupplies
{
    public class UnicornSuppliesContext : DbContext
    {
        public UnicornSuppliesContext(DbContextOptions<UnicornSuppliesContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
