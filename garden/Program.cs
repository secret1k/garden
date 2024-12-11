using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

// условная бд с пользователями
var people = new List<Person>
{
    new Person("tom@gmail.com", "12345"),
    new Person("bob@gmail.com", "55555")
};

var builder = WebApplication.CreateBuilder();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // добавление сервисов аутентификации
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true
        };
});
builder.Services.AddAuthorization();  // добавление сервисов авторизации

var app = builder.Build();

app.UseAuthentication();  // добавление middleware аутентификации
app.UseAuthorization();  // добавление middleware авторизации
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/login", (Person loginData) =>
{
    // находим пользователя
    Person? person = people.FirstOrDefault(p => p.Email == loginData.Email && p.Password == loginData.Password);
    // если пользователь не найден, отправляем статусный код 401
    if (person is null) return Results.Unauthorized();

    var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
    // создаем JWT-токен
    var jwt = new JwtSecurityToken(
        issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

    // формируем ответ
    var response = new
    {
        access_token = encodedJwt,
        username = person.Email
    };
    return Results.Json(response);
});
app.Map("/login/{username}", (string username) =>
{
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
    // создаем JWT-токен
    var jwt = new JwtSecurityToken(
        issuer: AuthOptions.ISSUER,
        audience: AuthOptions.AUDIENCE,
        claims: claims,
        expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
        signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(jwt);
});
app.Map("/data", [Authorize] () => new { message = "Hello world!" });


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

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer"; // издатель токена
    public const string AUDIENCE = "MyAuthClient";  // потребитель токена
    const string KEY = "myreallyreally_forrealhiddensecretmagickey!123456";  // ключ для шифрации
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
};

record class Person (string Email, string Password);