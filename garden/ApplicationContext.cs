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
        //modelBuilder.Entity<Order>()
        //    .HasOne(o => o.User)
        //    .WithMany(u => u.Orders)
        //    .HasForeignKey(o => o.UserId)
        //    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, Name = "cat1", Img = "none" },
            new Category { CategoryId = 2, Name = "cat2", Img = "none" }
            );
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "prod1", CategoryId = 1, Description = "good", Price = 100, Img = "none" },
            new Product { ProductId = 2, Name = "prod2", CategoryId = 1, Description = "mid", Price = 99, Img = "none" },
            new Product { ProductId = 3, Name = "prod3", CategoryId = 2, Description = "good", Price = 130, Img = "none" }
            );

        base.OnModelCreating(modelBuilder);
    }
}