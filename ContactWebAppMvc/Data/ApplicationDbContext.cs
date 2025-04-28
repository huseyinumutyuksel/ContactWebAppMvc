// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using ContactWebAppMvc.Models; // Modellerin bulunduğu namespace

namespace ContactWebAppMvc.Data // Proje adınıza göre namespace'i güncelleyin
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Veritabanında oluşturulacak tabloları temsil eden DbSet'ler
        public DbSet<Department> Departments { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }

        // İsteğe bağlı: Model oluşturulurken ek yapılandırmalar yapmak için
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);
        //
        //     // Örnek: Departman adının benzersiz (unique) olmasını sağlamak
        //     // modelBuilder.Entity<Department>()
        //     //     .HasIndex(d => d.Name)
        //     //     .IsUnique();
        // }
    }
}