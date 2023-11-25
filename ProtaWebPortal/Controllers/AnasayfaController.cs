using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtaWebPortal.Models;

namespace ProtaWebPortal.Controllers
{
	public class AnasayfaController : Controller
	{
		private readonly RezervasyonManager _rezervasyonManager;
		private readonly CihazManager _cihazManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly FiyatlandirmaManager _fiyalandirmaManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly EgitimManager _egitim;
		private readonly KullaniciveMentorEgitimleriManager _kullaniciveMentorManager;

		public AnasayfaController(RezervasyonManager rezervasyonManager, CihazManager cihazManager, UserManager<AppUser> userManager, FiyatlandirmaManager fiyatlandirmaManager,IHttpContextAccessor httpContextAccessor , EgitimManager egitim, KullaniciveMentorEgitimleriManager kullaniciveMentorEgitimleriManager)
		{
			_rezervasyonManager = rezervasyonManager;
			_cihazManager = cihazManager;
			_userManager = userManager;
			_fiyalandirmaManager = fiyatlandirmaManager;
			_httpContextAccessor = httpContextAccessor;
			_egitim = egitim;
			_kullaniciveMentorManager = kullaniciveMentorEgitimleriManager;
		}

		public async Task<IActionResult> Index()
		{
			var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
			
			var uyeSayisi = _userManager.Users.Count();
			ViewBag.uyeSayisi = uyeSayisi;
			
			var cihazSayisi = _cihazManager.TGetList().Count();
			ViewBag.cihazSayisi = cihazSayisi;

			DateTime dt = DateTime.Now;
			var rezervasyonSayisi = _rezervasyonManager.TGetList(x=>x.BaslangicTarihi.Date >= dt.Date).Count();
			ViewBag.rezervasyonSayisi = rezervasyonSayisi;
			
			var toplamFiyat = _fiyalandirmaManager.TGetList().Sum(x => x.ToplamFiyat);
			ViewBag.toplamFiyat = toplamFiyat;

			
	         var enCokKullanilanCihaz = _rezervasyonManager.TGetList().Include(x => x.cihaz)
	        .GroupBy(r => r.cihaz.CihazAdi)
	        .OrderByDescending(group => group.Count())
	        .Select(group => new { CihazAdi = group.Key, KullanımSayisi = group.Count() })
	        .FirstOrDefault();

			ViewBag.enCokKullanilanCihazAdi = enCokKullanilanCihaz?.CihazAdi;
			ViewBag.enCokKullanilanCihazKullanimSayisi = enCokKullanilanCihaz?.KullanımSayisi;


			var rezervasyonBul = _rezervasyonManager.TGetList(x => x.UserId == userId && x.BaslangicTarihi.Date>= dt.Date).Count();
			ViewBag.kullaniciAktifRezervasyon = rezervasyonBul;

			var rezervasyonFiyatBul = _fiyalandirmaManager.TGetList(x => x.appUserId == int.Parse(userId)).Sum(x => x.ToplamFiyat);
			ViewBag.kullaniciRezervasyonFiyati = rezervasyonFiyatBul;

			var egitimIstekleri = _egitim.TGetList(x => x.EgitimNeDurumda == "İstek gönderildi" && x.appUserId == int.Parse(userId)).Count();
			var devamedenEgitimler = _egitim.TGetList(x => x.EgitimNeDurumda == "Eğitim Başladı" && x.appUserId == int.Parse(userId)).Count();
			var tamamlananEgitim = _egitim.TGetList(x => x.EgitimNeDurumda == "Eğitim Onaylandı" && x.appUserId == int.Parse(userId)).Count();

			ViewBag.egitimistekleri = egitimIstekleri;
			ViewBag.devamedenegitim = devamedenEgitimler;
			ViewBag.tamamlananegitim = tamamlananEgitim;


			var kullaniciveMentor = _kullaniciveMentorManager.TGetList(x => x.AppUserId == int.Parse(userId)).Count();
			ViewBag.mentoregitimtoplam = kullaniciveMentor;

			var gelecekTarihtekiRezervasyonlar = _rezervasyonManager.TGetList(x => x.UserId == userId && x.BaslangicTarihi.Date > dt.Date)		
				.Select(x => new GelecekTarihtekiRezervasyonlar
				{					
					BaslangicTarihi = x.BaslangicTarihi,
					BitisTarihi = x.BitisTarihi,
					
				}).ToList();
			
			var rezervasyon_ = _rezervasyonManager.TGetList();

			var model1 = gelecekTarihtekiRezervasyonlar;
			var model2 = rezervasyon_;
			
			var modeller = (model1, model2);

			return View(modeller);
		}

		



	}
}
