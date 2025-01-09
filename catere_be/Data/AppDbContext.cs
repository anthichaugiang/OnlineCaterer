using catere_be.Dto;
using catere_be.Models;
using Microsoft.EntityFrameworkCore;

namespace catere_be.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<City> City { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Customer> Customer { get; set; }
       // public DbSet<CustomerDTO> Customer { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<SupplierDetail> SupplierDetail { get; set; }
        public DbSet<CustomerOrder> CustomerOrder { get; set; }
        public DbSet<CustomerInvoice> CustomerInvoice { get; set; }
        public DbSet<SupplierInvoice> SupplierInvoice { get; set; }
        public DbSet<CustomerOrderMenu> CustomerOrderMenu { get; set; }
        public DbSet<CustomerFeedback> CustomerFeedback { get; set; }
        public DbSet<Message> Message { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CustomerOrder>()
                .HasMany(co => co.CustomerOrderMenu)
                .WithOne(com => com.CustomerOrder)
                .HasForeignKey(com => com.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerOrderMenu>()
                .HasOne<Menu>()
                .WithMany()
                .HasForeignKey(com => com.MenuItemId);

            modelBuilder.Entity<CustomerOrderMenu>()
                .HasOne<Room>()
                .WithMany()
                .HasForeignKey(com => com.RoomId);
        }

    }
}
