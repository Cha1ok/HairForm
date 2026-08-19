using HairForm.Models;
using Microsoft.EntityFrameworkCore;

namespace HairForm.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderAccessory> OrderAccessories { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Связь: один заказ -> много аксессуаров
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Accessories)
                .WithOne(a => a.Order)
                .HasForeignKey(a => a.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // при удалении заказа удаляются аксессуары
        }
    }
}
