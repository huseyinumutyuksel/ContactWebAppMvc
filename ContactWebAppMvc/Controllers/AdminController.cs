// Controllers/AdminController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include ve ToListAsync için
using ContactWebAppMvc.Data; // DbContext için
using System.Linq;
using System.Threading.Tasks; // Async işlemler için

namespace ContactWebAppMvc.Controllers // Namespace'i kontrol edin
{
    // TODO: [Authorize(Roles = "Admin")] // Gerçek uygulamada buraya yetkilendirme eklenmeli
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin veya /Admin/Index
        // Tüm iletişim mesajlarını listelemek için action
        public async Task<IActionResult> Index()
        {
            // Tüm mesajları çek, Departman bilgisini de Include ile getir (.Include() için Microsoft.EntityFrameworkCore gerekir)
            // En son gönderilen mesaj en üstte olacak şekilde sırala
            var messages = await _context.ContactMessages
                                         .Include(m => m.Department) // İlişkili Departman verisini de yükle
                                         .OrderByDescending(m => m.SentAt) // Gönderim zamanına göre tersten sırala
                                         .ToListAsync(); // Asenkron olarak listeye çevir

            return View(messages); // Mesaj listesini View'a model olarak gönder
        }

        // GET: /Admin/Details/5
        // Tek bir mesajın detayını görmek için (Opsiyonel - İleride eklenebilir)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contactMessage = await _context.ContactMessages
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Id == id); // Belirtilen ID'ye sahip mesajı bul

            if (contactMessage == null)
            {
                return NotFound();
            }

            return View(contactMessage); // Detay View'ına gönder (Bu View'ı oluşturmanız gerekir)
        }

        // TODO: Mesaj silme, işaretleme gibi diğer Admin action'ları buraya eklenebilir.

    }
}