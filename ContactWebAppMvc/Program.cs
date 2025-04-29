// Program.cs (.NET 6 ve sonras�)

using Microsoft.EntityFrameworkCore; // DbContext i�in
using ContactWebAppMvc.Data;     // ApplicationDbContext i�in

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Veritaban� Ba�lant�s�n� Yap�land�r
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); // Ba�lant� dizesi bulunamazsa hata f�rlat


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies() // Lazy loading proxy'lerini etkinle�tirir
           .UseSqlServer(connectionString)); // SQL Server kullanaca��m�z� belirtiyoruz


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();