using Microsoft.EntityFrameworkCore;
public class ApplicationContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();   // создаем базу данных при первом обращении
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, Name = "prod1", CategoryId = 1, Description = "good", Price = 100, Img = "none" },
            new Product { ProductId = 2, Name = "prod2", CategoryId = 1, Description = "mid", Price = 99, Img = "none" },
            new Product { ProductId = 3, Name = "prod3", CategoryId = 2, Description = "good", Price = 130, Img = "none" }
            );
    }
}