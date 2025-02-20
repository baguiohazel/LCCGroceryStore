using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiWithRoleAuthentication.Models;

namespace WebApiWithRoleAuthentication.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<UserRole> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "InventoryManager", NormalizedName = "INVENTORYMANAGER" },
                new IdentityRole { Id = "3", Name = "Cashier", NormalizedName = "CASHIER" }
            );

            // Seed Users
            var hasher = new PasswordHasher<IdentityUser>();

            var adminUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(), // Add ID to match role assignment
                UserName = "admin@admin.com",
                NormalizedUserName = "ADMIN@ADMIN.COM",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                PhoneNumber = "13024984",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false
            };
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "admin123");

            var inventoryManagerUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "manager@manager.com",
                NormalizedUserName = "MANAGER@MANAGER.COM",
                Email = "manager@manager.com",
                NormalizedEmail = "MANAGER@MANAGER.COM",
                PhoneNumber = "13024984",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false
            };
            inventoryManagerUser.PasswordHash = hasher.HashPassword(inventoryManagerUser, "manager123");

            var cashierUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "cashier@cashier.com",
                NormalizedUserName = "CASHIER@CASHIER.COM",
                Email = "cashier@cashier.com",
                NormalizedEmail = "CASHIER@CASHIER.COM",
                PhoneNumber = "1374987",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                LockoutEnabled = false
            };
            cashierUser.PasswordHash = hasher.HashPassword(cashierUser, "cashier123");

            builder.Entity<IdentityUser>().HasData(adminUser, inventoryManagerUser, cashierUser);

            // Assign Roles to Users
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = "1", UserId = adminUser.Id },
                new IdentityUserRole<string> { RoleId = "2", UserId = inventoryManagerUser.Id },
                new IdentityUserRole<string> { RoleId = "3", UserId = cashierUser.Id }
            );

            // Configure relationships
            builder.Entity<Product>()
                .HasOne(p => p.Categories)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            builder.Entity<Product>()
                .HasOne(p => p.Suppliers)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId);

            builder.Entity<Order>()
                .HasOne(o => o.Customers)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            builder.Entity<Order>()
                .HasOne(o => o.Users)
                .WithMany() // Assuming IdentityUser doesn't have navigation property for Orders
                .HasForeignKey(o => o.UserId);

            
           builder.Entity<OrderDetail>()  
                .HasOne(od => od.Orders)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);
               

            builder.Entity<OrderDetail>()
                .HasOne(od => od.Products)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
