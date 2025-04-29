// Program.cs (.NET 6 ve sonrasý)

using Microsoft.EntityFrameworkCore; // DbContext için
using ContactWebAppMvc.Data;     // ApplicationDbContext için

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. Veritabaný Baðlantýsýný Yapýlandýr
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found."); // Baðlantý dizesi bulunamazsa hata fýrlat


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseLazyLoadingProxies() // Lazy loading proxy'lerini etkinleþtirir
           .UseSqlServer(connectionString)); // SQL Server kullanacaðýmýzý belirtiyoruz


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