using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
builder.Services.AddControllers();
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

app.MapControllers();
app.MapPost("/login", (Person loginData) =>
{
    //// находим пользователя
    //Person? person = people.FirstOrDefault(p => p.Email == loginData.Email && p.Password == loginData.Password);
    //// если пользователь не найден, отправляем статусный код 401
    //if (person is null) return Results.Unauthorized();

    //var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
    //// создаем JWT-токен
    //var jwt = new JwtSecurityToken(
    //    issuer: AuthOptions.ISSUER,
    //    audience: AuthOptions.AUDIENCE,
    //    claims: claims,
    //    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
    //    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
    //var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

    //// формируем ответ
    //var response = new
    //{
    //    access_token = encodedJwt,
    //    username = person.Email
    //};
    //return Results.Json(response);
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

//app.Map("/data", [Authorize] () => new { message = "Hello world!" });

app.MapGet("/api/categories", async (ApplicationContext db) => await db.Categories.ToListAsync());

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