using Microsoft.EntityFrameworkCore;
using TransvitiTest.Models;

namespace TransvitiTest.DBFolder
{
    public class EcommerceDbContext : DbContext
    {
        public EcommerceDbContext(DbContextOptions options) : base (options)
        {
        }

        public DbSet<Products> Products { get; set; }
    }
}
