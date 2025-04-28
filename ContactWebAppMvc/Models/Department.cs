// Models/Department.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactWebAppMvc.Models // Proje adınıza göre namespace'i güncelleyin
{
    public class Department
    {
        [Key] // Primary Key olduğunu belirtir
        public int Id { get; set; }

        [Required(ErrorMessage = "Departman adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Departman adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Departman Adı")] // Formlarda görünecek etiket adı
        public string Name { get; set; }

        // İlişkili Mesajlar (Bir departmanın birden çok mesajı olabilir)
        // virtual anahtar kelimesi Lazy Loading'i etkinleştirmek için kullanılabilir.
        public virtual ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>(); // Null referans hatası almamak için koleksiyonu başlatıyoruz.
    }
}