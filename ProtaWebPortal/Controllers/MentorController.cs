using BusinessLayer.Concrete;
using DataAccess.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtaWebPortal.Models;
using X.PagedList;

namespace ProtaWebPortal.Controllers
{
	public class MentorController : Controller
	{
		private readonly MentorManager _mentor;
		private readonly IHttpContextAccessor _http;
		private readonly UserManager<AppUser> _userManager;
		private readonly ProtaDbContext _c;
		public MentorController(MentorManager mentor, IHttpContextAccessor http, UserManager<AppUser> userManager,ProtaDbContext c)
		{
			_mentor = mentor;
			_http = http;
			_userManager = userManager;
			_c = c;	
		}
		public IActionResult Index(int page = 1)
		{
			var values = _mentor.TGetList()
				.Select(x => new MentorListeModel
				{
					Id = x.Id,
					Adi = x.Adi,
					Soyadi = x.Soyadi,
					EMail = x.Email,
					UzmanlikAlani = x.UzmanlikAlani,					
					KayitTarihi = x.KayitTarihi,					
				}).ToPagedList(page, 20);

			var mentorListe = _mentor.TGetList().ToPagedList(page , 10);
			var model = (values, mentorListe);
			return View(model);
		}

		[HttpGet]
		public IActionResult MentorEkle()
		{
			return View();
		}
		[HttpPost]
		public IActionResult MentorEkle(Mentor mentor)
		{
			var username = _http.HttpContext.Session.GetString("UserName");
			Mentor m = new Mentor();
			m.Adi = mentor.Adi;
			m.Soyadi = mentor.Soyadi;
			m.Email = mentor.Email;
			m.KayitTarihi = DateTime.Now;
			m.UzmanlikAlani = mentor.UzmanlikAlani;
			//m.AppUserId = mentor.AppUserId;
			_mentor.TAdd(m);

			string logMessage = "Yeni mentor eklendi " + "İşlemi yapan kullanıcı adı: " + " " + username + " " + " Tarih: " + DateTime.Now;
			string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

			using (StreamWriter sw = new StreamWriter(logFilePath, true))
			{
				sw.WriteLine(logMessage);
			}
			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Duzenle(int id)
		{
			var values = _mentor.TGetById(id);
			return View(values);
		}
		[HttpPost]
		public IActionResult Duzenle(int id , Mentor model)
		{
			var username = _http.HttpContext.Session.GetString("UserName");
			if (model != null)
			{
				var values = _mentor.TGetById(id);
				values.Adi = model.Adi;
				values.Soyadi = model.Soyadi;
				values.Email = model.Email;
				values.UzmanlikAlani = model.UzmanlikAlani;				
				values.KayitTarihi = values.KayitTarihi;

				_mentor.TUpdate(values);
				string logMessage = "Mnetor bilgileri düzenlendi. " + "İşlemi yapan kullanıcı adı: " + " " + username + " " + " Tarih: " + DateTime.Now;
				string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

				using (StreamWriter sw = new StreamWriter(logFilePath, true))
				{
					sw.WriteLine(logMessage);
				}
			}
			return RedirectToAction("Index");
		}

		public IActionResult Sil(int id)
		{
			var username = _http.HttpContext.Session.GetString("UserName");
			var values = _mentor.TGetById(id);
			_mentor.TDelete(values);
			string logMessage = "Mnetor bilgileri silindi. " + "İşlemi yapan kullanıcı adı: " + " " + username + " " + " Tarih: " + DateTime.Now;
			string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

			using (StreamWriter sw = new StreamWriter(logFilePath, true))
			{
				sw.WriteLine(logMessage);
			}
			return RedirectToAction("Index");
		}

		public IActionResult KullaniciListe()
		{
			var values = _c.Users.Select(x => new UyeListeModel
			{
				Id = x.Id,
				Adi = x.Isim,
				Soyadi = x.Soyisim,

			}).AsQueryable();
			return Json(values);
		}
	}
}
