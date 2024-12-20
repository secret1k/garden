using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
public class ApplicationContext : DbContext
{
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Review> Reviews { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderItem> Items { get; set; } = null!;

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        // EnsureCreate заменен на миграции
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // настройка связей
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "category1", Img = "none" },
            new Category { CategoryId = 2, Name = "category2", Img = "none" }
            );
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "prod1", CategoryId = 1, Description = "good", Price = 100, Img = "none" },
            new Product { ProductId = 2, Name = "prod2", CategoryId = 1, Description = "mid", Price = 99, Img = "none" },
            new Product { ProductId = 3, Name = "prod3", CategoryId = 2, Description = "good", Price = 130, Img = "none" }
            );
        modelBuilder.Entity<User>().HasData(
            new User { UserId = 1, Name = "User1", Email = "user1@example.com", Password = "password", Role = "Customer" },
            new User { UserId = 2, Name = "User2", Email = "user2@example.com", Password = "password", Role = "Customer" }
            );
        modelBuilder.Entity<Review>().HasData(
            new Review { ReviewId = 1, ProductId = 1, UserId = 1, Comment = "comment1", Rating = 5, CreatedAt = new DateTime(2024, 12, 20) },
            new Review { ReviewId = 2, ProductId = 2, UserId = 2, Comment = "comment2", Rating = 3, CreatedAt = new DateTime(2024, 12, 20) }
            );

        base.OnModelCreating(modelBuilder);
    }
}