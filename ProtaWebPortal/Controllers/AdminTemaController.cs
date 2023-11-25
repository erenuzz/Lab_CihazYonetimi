using Microsoft.AspNetCore.Mvc;

namespace ProtaWebPortal.Controllers
{
    public class AdminTemaController : Controller
    {
        public PartialViewResult Header()
        {
            return PartialView();
        }

        public PartialViewResult Navbar() 
        {
            return PartialView();
        }

        public PartialViewResult SideBar()
        {
            return PartialView();
        }

        public PartialViewResult Script()
        {
            return PartialView();
        }
    }
}
