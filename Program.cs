using CryptoWallet.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Konfiguracja JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.WebHost.UseUrls("http://localhost:7052");

builder.Services.AddAuthorization(); // Włącza obsługę autoryzacji

// Dodanie kontroli API
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext"))
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // Pozwól na dostęp z portu 3000 (frontend)
              .AllowAnyMethod()                    // Pozwól na różne metody HTTP (GET, POST, PUT, DELETE)
              .AllowAnyHeader();                   // Pozwól na dowolne nagłówki
    });
});

var app = builder.Build();
app.UseCors("AllowReactApp");

// Obsługa JWT
app.UseAuthentication(); // Włączenie uwierzytelniania
app.UseAuthorization();  // Włączenie autoryzacji

// Obsługa wyjątków i HTTPS
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowReactApp");
// Włączenie CORS dla Reacta

// Mapowanie kontrolerów API
app.MapControllers(); // Rejestruje API (np. WalletController)

// Obsługa Reacta jako SPA
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "clientapp";

    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
    }
});


app.Run();
