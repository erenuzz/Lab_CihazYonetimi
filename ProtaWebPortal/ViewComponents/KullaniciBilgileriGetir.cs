using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ProtaWebPortal.ViewComponents
{
    public class KullaniciBilgileriGetir:ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public KullaniciBilgileriGetir(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            var userName = HttpContext.Session.GetString("AdSoyad");
            var rol = HttpContext.Session.GetString("UserRole");
            ViewBag.rol = rol;
            ViewBag.UserName = userName;
            return View();
        }
    }
}
