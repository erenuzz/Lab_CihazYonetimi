using DataAccess.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProtaWebPortal.Models;

namespace ProtaWebPortal.Controllers
{
    public class KayitController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ProtaDbContext _c;

        public KayitController(UserManager<AppUser> userManager , ProtaDbContext c)
        {
            _userManager = userManager;
            _c = c;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(KullaniciKayitModel model) 
        {
            if (ModelState.IsValid)
            {
                Mail mail = new Mail();
                AppUser appUser = new AppUser()
                {
                    Isim = model.Adi,
                    Soyisim = model.Soyadi,
                    UserName = model.Email,
                    KayitTarihi = DateTime.Now,
                    Durum = true,
                    KullaniciTurleriId = 2
                    
                };
                var email = _c.Users.FirstOrDefault(x=>x.UserName == model.Email);
                if (email == null)
                {
                    var result = await _userManager.CreateAsync(appUser, model.Sifre);
                    if (result.Succeeded)
                    {
                        var rol = await _userManager.AddToRoleAsync(appUser, "Standart Kullanici");
                        TempData["KayıtBasarili"] = "Kaydınız başarılı bir şekilde gerçekleşti";
                       
                        string logMessage = "Kayıt Yapıldı: " + "Kayıt olan kullanıcının adı: " + " " + model.Adi + " " + model.Soyadi + " " + " Tarih: " + DateTime.Now;
                        string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

                        using (StreamWriter sw = new StreamWriter(logFilePath, true))
                        {
                            sw.WriteLine(logMessage);
                        }

                        mail.MailGonder("Yeni Kayıt", model.Email, model.Adi + " " + model.Soyadi, $"Kaydınız başarılı bir şekilde gerçekleşti.");

                        return RedirectToAction("Index", "Giris");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError("", item.Description);
                        }
                    }
                }
                else
                {
                    ViewBag.email = "Kayıtlı olan bir email adresi ile tekrar kayıt olamazsınız";
                }
                
                return View();
            }
            return View();
        }
    }
}
