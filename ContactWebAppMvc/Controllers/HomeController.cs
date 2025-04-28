// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include için eklendi
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList için eklendi
using ContactWebAppMvc.Data; // DbContext için
using ContactWebAppMvc.Models; // Modeller için
using System.Diagnostics;
using System.Linq; // FirstOrDefaultAsync, ToListAsync için
using System.Threading.Tasks; // Async iþlemler için

namespace ContactWebAppMvc.Controllers // Namespace'i kontrol edin
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // DbContext'i ekledik

        // Constructor Injection ile DbContext'i alýyoruz
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Gelen context'i deðiþkene atýyoruz
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // GET: /Home/Contact
        // Ýletiþim formunu göstermek için action
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            // Departmanlarý veritabanýndan çekip SelectList'e dönüþtürerek ViewBag'e atýyoruz
            // Bu, dropdown'ý doldurmak için kullanýlacak.
            ViewBag.Departments = new SelectList(await _context.Departments.OrderBy(d => d.Name).ToListAsync(), "Id", "Name");

            // Boþ bir ContactMessage modeli ile View'ý döndürüyoruz
            return View(new ContactMessage());
        }

        // POST: /Home/Contact
        // Formdan gönderilen verileri iþlemek için action
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF saldýrýlarýný önlemek için
        public async Task<IActionResult> Contact(ContactMessage contactMessage)
        {
            // Model validasyonunu kontrol et (Required, StringLength vb. kurallara uyuyor mu?)
            if (ModelState.IsValid)
            {
                try
                {
                    // contactMessage.SentAt zaten modelde default olarak ayarlanýyor,
                    // ama isterseniz burada tekrar set edebilirsiniz:
                    // contactMessage.SentAt = DateTime.Now;

                    _context.Add(contactMessage); // Yeni mesajý DbContext'e ekle
                    await _context.SaveChangesAsync(); // Deðiþiklikleri veritabanýna kaydet

                    // Baþarýlý kayýt sonrasý kullanýcýya mesaj göster
                    TempData["SuccessMessage"] = "Mesajýnýz baþarýyla gönderildi. En kýsa sürede ilgili departmanýmýz sizinle iletiþime geçecektir.";

                    // Baþarýlý iþlem sonrasý Index sayfasýna veya özel bir 'Tesekkurler' sayfasýna yönlendir
                    return RedirectToAction(nameof(Index));
                    // Alternatif: return RedirectToAction("Tesekkurler"); (Tesekkurler action'ý ve view'ý oluþturmanýz gerekir)
                }
                catch (DbUpdateException ex)
                {
                    // Veritabaný kaydý sýrasýnda bir hata olursa logla ve kullanýcýya hata mesajý göster
                    _logger.LogError(ex, "Mesaj kaydedilirken veritabaný hatasý oluþtu.");
                    ModelState.AddModelError("", "Mesajýnýz gönderilirken bir hata oluþtu. Lütfen daha sonra tekrar deneyiniz.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Mesaj gönderilirken beklenmedik bir hata oluþtu.");
                    ModelState.AddModelError("", "Mesajýnýz gönderilirken beklenmedik bir hata oluþtu.");
                }
            }

            // Eðer ModelState.IsValid == false ise (yani validasyon hatasý varsa)
            // VEYA try-catch içinde bir hata oluþtuysa:
            // Formu tekrar gösterirken Departman dropdown'ýný yeniden doldurmamýz GEREKÝR!
            ViewBag.Departments = new SelectList(await _context.Departments.OrderBy(d => d.Name).ToListAsync(), "Id", "Name", contactMessage.DepartmentId); // Seçili deðeri koru

            // Hatalarla birlikte ayný View'ý tekrar göster
            return View(contactMessage);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}