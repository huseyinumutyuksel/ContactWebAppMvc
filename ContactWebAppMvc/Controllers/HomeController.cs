// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Include i�in eklendi
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList i�in eklendi
using ContactWebAppMvc.Data; // DbContext i�in
using ContactWebAppMvc.Models; // Modeller i�in
using System.Diagnostics;
using System.Linq; // FirstOrDefaultAsync, ToListAsync i�in
using System.Threading.Tasks; // Async i�lemler i�in

namespace ContactWebAppMvc.Controllers // Namespace'i kontrol edin
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // DbContext'i ekledik

        // Constructor Injection ile DbContext'i al�yoruz
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Gelen context'i de�i�kene at�yoruz
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        private async Task PopulateDepartmentsDropdown(int? selectedDepartmentId = null)
        {
            var departments = await _context.Departments.OrderBy(d => d.Name).ToListAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name", selectedDepartmentId);
        }


        // GET: /Home/Contact
        // �leti�im formunu g�stermek i�in action
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            // Departmanlar� veritaban�ndan �ekip SelectList'e d�n��t�rerek ViewBag'e at�yoruz
            // Bu, dropdown'� doldurmak i�in kullan�lacak.
      
            await PopulateDepartmentsDropdown();
            // Bo� bir ContactMessage modeli ile View'� d�nd�r�yoruz
            return View(new ContactMessage());
        }

        // POST: /Home/Contact
        // Formdan g�nderilen verileri i�lemek i�in action
        [HttpPost]
        [ValidateAntiForgeryToken] // CSRF sald�r�lar�n� �nlemek i�in
        public async Task<IActionResult> Contact([Bind("Id,DepartmentId,FullName,Email,PhoneNumber,Message,SentAt")] ContactMessage contactMessage)
        {
            // Model validasyonunu kontrol et (Required, StringLength vb. kurallara uyuyor mu?)
            if (ModelState.IsValid)
            {
                try
                {
                    // contactMessage.SentAt zaten modelde default olarak ayarlan�yor,
                    // ama isterseniz burada tekrar set edebilirsiniz:
                    // contactMessage.SentAt = DateTime.Now;

                    _context.Add(contactMessage); // Yeni mesaj� DbContext'e ekle
                    await _context.SaveChangesAsync(); // De�i�iklikleri veritaban�na kaydet

                    // Ba�ar�l� kay�t sonras� kullan�c�ya mesaj g�ster
                    TempData["SuccessMessage"] = "Mesaj�n�z ba�ar�yla g�nderildi. En k�sa s�rede ilgili departman�m�z sizinle ileti�ime ge�ecektir.";

                    // Ba�ar�l� i�lem sonras� Index sayfas�na veya �zel bir 'Tesekkurler' sayfas�na y�nlendir
                    return RedirectToAction(nameof(Index));
                    // Alternatif: return RedirectToAction("Tesekkurler"); (Tesekkurler action'� ve view'� olu�turman�z gerekir)
                }
                catch (DbUpdateException ex)
                {
                    // Veritaban� kayd� s�ras�nda bir hata olursa logla ve kullan�c�ya hata mesaj� g�ster
                    _logger.LogError(ex, "Mesaj kaydedilirken veritaban� hatas� olu�tu.");
                    ModelState.AddModelError("", "Mesaj�n�z g�nderilirken bir hata olu�tu. L�tfen daha sonra tekrar deneyiniz.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Mesaj g�nderilirken beklenmedik bir hata olu�tu.");
                    ModelState.AddModelError("", "Mesaj�n�z g�nderilirken beklenmedik bir hata olu�tu.");
                }

                foreach (var state in ModelState)
                {
                    if (state.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                    {
                        _logger.LogError($"Field {state.Key} is invalid. Errors: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

            }

            // E�er ModelState.IsValid == false ise (yani validasyon hatas� varsa)
            // VEYA try-catch i�inde bir hata olu�tuysa:
            // Formu tekrar g�sterirken Departman dropdown'�n� yeniden doldurmam�z GEREK�R!
            await PopulateDepartmentsDropdown(contactMessage.DepartmentId);

            // Hatalarla birlikte ayn� View'� tekrar g�ster
            return View(contactMessage);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}