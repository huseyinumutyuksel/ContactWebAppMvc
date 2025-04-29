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
        private readonly ILogger<AdminController> _logger; // Logger değişkeni

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger; // Logger'ı kullanmak için private değişken
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
                _logger.LogWarning("Contact message with ID {MessageId} not found.", id); // Opsiyonel loglama
                return NotFound();
            }

            return View(contactMessage); // Detay View'ına gönder (Bu View'ı oluşturmanız gerekir)
        }

        // GET: /Admin/Delete/5
        // Silme onay sayfasını göstermek için
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Delete GET action called with null ID.");
                return NotFound();
            }

            // Mesajı ID'ye göre bul, Departman bilgisini de yükle (onay sayfasında göstermek isterseniz)
            var contactMessage = await _context.ContactMessages
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contactMessage == null)
            {
                _logger.LogWarning("Contact message with ID {MessageId} not found for deletion.", id);
                return NotFound();
            }

            // Onay View'ına modeli gönder
            return View(contactMessage);
        }

        // POST: /Admin/Delete/5
        // Silme işlemini gerçekleştirmek için
        [HttpPost, ActionName("Delete")] // Formdan gelen POST isteğini bu metoda yönlendirir (ActionName önemli)
        [ValidateAntiForgeryToken] // CSRF saldırılarını önler
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation("Delete POST action called for ID {MessageId}.", id);
            var contactMessage = await _context.ContactMessages.FindAsync(id);

            if (contactMessage == null)
            {
                _logger.LogWarning("Attempted to delete non-existent message with ID {MessageId}.", id);
                return NotFound(); // Mesaj bulunamadıysa hata ver
            }

            try
            {
                _context.ContactMessages.Remove(contactMessage); // Mesajı silmek üzere işaretle
                await _context.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet (asıl silme burada olur)
                _logger.LogInformation("Successfully deleted message with ID {MessageId}.", id);

                // Başarı mesajı ekle (opsiyonel)
                TempData["SuccessMessage"] = "Mesaj başarıyla silindi.";

                return RedirectToAction(nameof(Index)); // Listeleme sayfasına geri yönlendir
            }
            catch (DbUpdateException ex)
            {
                // Veritabanı silme hatası olursa logla ve hata göster
                _logger.LogError(ex, "Error occurred while deleting message with ID {MessageId}.", id);
                // Hata mesajını kullanıcıya göstermek için TempData veya ModelState kullanılabilir
                TempData["ErrorMessage"] = "Mesaj silinirken bir veritabanı hatası oluştu. İlişkili kayıtlar olabilir.";
                // VEYA: ModelState.AddModelError("", "Mesaj silinirken bir hata oluştu.");
                // Kullanıcıyı tekrar onay sayfasına veya listeye yönlendirebilirsiniz.
                // Hata durumunda onay sayfasına geri dönmek daha mantıklı olabilir:
                return RedirectToAction(nameof(Delete), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting message with ID {MessageId}.", id);
                TempData["ErrorMessage"] = "Mesaj silinirken beklenmedik bir hata oluştu.";
                return RedirectToAction(nameof(Index));
            }
        }
        // TODO: Mesaj silme, işaretleme gibi diğer Admin action'ları buraya eklenebilir.

    }
}