using Microsoft.EntityFrameworkCore;

namespace Test.RabbitMQ.API.Models
{
    public class InventoryContext : DbContext
    {
        public InventoryContext()
        {

        }

        public InventoryContext(DbContextOptions<InventoryContext> options) : base(options)
        { }

        public DbSet<Products> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432; Username=postgres; Password=admin; Database=inventory_db; Integrated Security=true;Pooling=true;");
            }
        }
    }
}
