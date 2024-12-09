using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder();
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection)); 

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/products", async (ApplicationContext db) => await db.Products.ToListAsync());

app.MapGet("/api/products/{id:int}", async (int id, ApplicationContext db) =>
{
    Product? product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
    if (product == null) return Results.NotFound(new { message = "товар не найден" });
    return Results.Json(product);
});

app.MapDelete("/api/products/{id:int}", async (int id, ApplicationContext db) =>
{
    Product? product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == id);
    if (product == null) return Results.NotFound(new { message = "товар не найден" });
    db.Products.Remove(product);
    await db.SaveChangesAsync();
    return Results.Json(product);
});

app.MapPost("/api/products/", async (Product product, ApplicationContext db) =>
{
    await db.Products.AddAsync(product);
    await db.SaveChangesAsync();
    return product;
});

app.MapPut("/api/products", async (Product ProductData, ApplicationContext db) =>
{
    var product = await db.Products.FirstOrDefaultAsync(p => p.ProductId == ProductData.ProductId);
    if (product == null) return Results.NotFound(new { message = "товар не найден" });
    product.Name = ProductData.Name;
    product.CategoryId = ProductData.CategoryId;
    product.Description = ProductData.Description;
    product.Price = ProductData.Price;
    product.Img = ProductData.Img;
    await db.SaveChangesAsync();
    return Results.Json(product);
});

//app.MapGet("/", (ApplicationContext db) => db.Products.ToList());

app.Run();
