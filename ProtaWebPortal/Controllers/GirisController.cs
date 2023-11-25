using EntityLayer.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProtaWebPortal.Models;

namespace ProtaWebPortal.Controllers
{
    public class GirisController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public GirisController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(KullaniciGirisModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, true);
                if (result.Succeeded)
                {
                    var kullanici = await _userManager.FindByNameAsync(model.UserName);                  
                    HttpContext.Session.SetString("UserId", kullanici.Id.ToString());
                    HttpContext.Session.SetString("UserName", kullanici.UserName);
                    HttpContext.Session.SetString("AdSoyad", kullanici.Isim + " " + kullanici.Soyisim);

                    var roles = await _userManager.GetRolesAsync(kullanici);
                    HttpContext.Session.SetString("UserRole", roles.FirstOrDefault());

                    string logMessage = "Giriş Yapıldı: " + "Giriş yapan kullanıcının adı: " + " " + kullanici.Isim + " " + kullanici.Soyisim  + " " + " Tarih: " + DateTime.Now;
                    string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

                    using (StreamWriter sw = new StreamWriter(logFilePath, true))
                    {
                        sw.WriteLine(logMessage);
                    }
                    return Redirect("/Anasayfa/Index/");
                }
                else
                {
                    ViewBag.hata = "Kullanıcı bulunamadı";
                }
            }
           
            return View();
        }

        public async Task<IActionResult> CikisYap()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Giris/Index/");
        }
    }
}
