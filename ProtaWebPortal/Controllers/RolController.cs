using DataAccess.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtaWebPortal.Models;

namespace ProtaWebPortal.Controllers
{

    public class RolController : Controller
    {
        private readonly RoleManager<AppRole> _role;
        private readonly ProtaDbContext _c;

        public RolController(RoleManager<AppRole> role, ProtaDbContext c)
        {
            _role = role;
            _c = c;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult RolEkle()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RolEkle(RolEkleModel model)
        {
            if (ModelState.IsValid)
            {
                AppRole role = new AppRole()
                {
                    Name = model.Name
                };

                var result = await _role.CreateAsync(role);
                if (result.Succeeded)
                {
                    _c.SaveChanges();
                    return RedirectToAction("RolEkle");
                }
            }
            return View(model);


        }

    }
}
