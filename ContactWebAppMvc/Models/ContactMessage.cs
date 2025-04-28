// Models/ContactMessage.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // ForeignKey için gerekli

namespace ContactWebAppMvc.Models // Proje adına göre namespace'i güncelle
{
    public class ContactMessage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Adı Soyadı alanı zorunludur.")]
        [StringLength(100, ErrorMessage = "Adı Soyadı en fazla 100 karakter olabilir.")]
        [Display(Name = "Adınız Soyadınız")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Telefon numarası alanı zorunludur.")]
        [Phone(ErrorMessage = "Lütfen geçerli bir telefon numarası formatı giriniz.")]
        [StringLength(15, ErrorMessage = "Telefon numarası en fazla 15 karakter olabilir.")]
        [Display(Name = "Telefon Numaranız")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "E-posta adresi alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta adresi giriniz.")]
        [StringLength(100, ErrorMessage = "E-posta adresi en fazla 100 karakter olabilir.")]
        [Display(Name = "E-posta Adresiniz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mesaj alanı zorunludur.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Mesajınız en az 10, en fazla 1000 karakter olabilir.")]
        [DataType(DataType.MultilineText)] // View'da textarea olarak render edilmesine yardımcı olur
        [Display(Name = "Mesajınız")]
        public string Message { get; set; }

        [Display(Name = "Gönderim Zamanı")]
        public DateTime SentAt { get; set; } = DateTime.Now; // Mesaj oluşturulduğunda otomatik olarak zamanı ata

        // Foreign Key - Department İlişkisi
        [Required(ErrorMessage = "Lütfen bir departman seçiniz.")]
        [Display(Name = "Departman")]
        public int DepartmentId { get; set; } // Foreign Key sütunu

        // Navigation Property - İlişkili Departmanı temsil eder
        [ForeignKey("DepartmentId")] // Hangi özelliğin Foreign Key olduğunu belirtir
        public virtual Department Department { get; set; } // virtual Lazy Loading için
    }
}