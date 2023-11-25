using BusinessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtaWebPortal.Models;
using X.PagedList;

namespace ProtaWebPortal.Controllers
{
    public class MentorEgitimleriController : Controller
    {
        private readonly MentorEgitimManager _mentoregitim;
        private readonly IHttpContextAccessor _http;
        private readonly MentorManager _mentor;
        private readonly KullaniciveMentorEgitimleriManager _kullanicivementormanager;
        public MentorEgitimleriController(MentorEgitimManager mentorEgitimManager, IHttpContextAccessor http, MentorManager mentor, KullaniciveMentorEgitimleriManager kullaniciveMentorEgitimleriManager)
        {
            _mentoregitim = mentorEgitimManager;
            _http = http;
            _mentor = mentor;
            _kullanicivementormanager = kullaniciveMentorEgitimleriManager;
        }


        #region Index
        public IActionResult Index(int page = 1)
        {
            var userId = _http.HttpContext.Session.GetString("UserId");
            var values = _mentoregitim.TGetList().Include(x => x.Mentor)
                .Select(x => new MentorEgitimleriModel
                {
                    Id = x.Id,
                    EgitimAdi = x.EgitimAdi,
                    Suresi = x.Suresi,
                    BaslangicTarihi = x.BaslangicTarihi,
                    BitisTarihi = x.BitisTarihi,
                    EgitimKapakFotografi = x.EgitimKapakFotografi,
                    MentorAdi = x.Mentor.Adi + " " + x.Mentor.Soyadi,
                    MentorId = x.MentorId,
                    Kontenjan = x.Kontenjan
                }).ToPagedList(page, 20);

            var egitimBilgileri = _kullanicivementormanager.TGetList(x => x.AppUserId == int.Parse(userId)).Include(x => x.MentorEgitimleri)
                .Select(x => new KullaniciVeMentorEgitimleriModel
                {
                    Id = x.Id,
                    EgitimAdi = x.MentorEgitimleri.EgitimAdi,
                    BaslangicTarihi = x.MentorEgitimleri.BaslangicTarihi,
                    BitisTarihi = x.MentorEgitimleri.BitisTarihi,
                    Suresi = x.MentorEgitimleri.Suresi,
                }).ToPagedList(page, 20);

            var egitimlerimList = egitimBilgileri;
            var mentorList = _mentor.TGetList().ToPagedList(page, 10);
            var model = (values, mentorList , egitimlerimList);
            return View(model);
        }
        #endregion

        #region Ekleme
        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Ekle(MentorEgitimleri ment, IFormFile kapakGorsel)
        {
            var userName = _http.HttpContext.Session.GetString("UserName");
            MentorEgitimleri m = new MentorEgitimleri();

            if (kapakGorsel != null)
            {
                var extension = Path.GetExtension(kapakGorsel.FileName);
                var newimagename = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MentorEgitimGorselleri/", newimagename);
                var stream = new FileStream(location, FileMode.Create);
                kapakGorsel.CopyTo(stream);
                m.EgitimKapakFotografi = "/MentorEgitimGorselleri/" + newimagename;
            }

            m.Kontenjan = ment.Kontenjan;
            m.EgitimAdi = ment.EgitimAdi;
            m.Suresi = ment.Suresi;
            m.BaslangicTarihi = ment.BaslangicTarihi;
            m.BitisTarihi = ment.BitisTarihi;
            _mentoregitim.TAdd(m);
            string logMessage = "Yeni mentor eğitimi eklendi: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Duzenle
        [HttpGet]
        public IActionResult Duzenle(int id)
        {
            var values = _mentoregitim.TGetById(id);
            return View(values);
        }

        [HttpPost]
        public IActionResult Duzenle(int id, MentorEgitimleri model, IFormFile kapakGorsel)
        {
            string userName = _http.HttpContext.Session.GetString("UserName");
            var values = _mentoregitim.TGetById(id);
            if (kapakGorsel != null)
            {
                var extension = Path.GetExtension(kapakGorsel.FileName);
                var newimagename = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/MentorEgitimGorselleri/", newimagename);
                var stream = new FileStream(location, FileMode.Create);
                kapakGorsel.CopyTo(stream);
                values.EgitimKapakFotografi = "/MentorEgitimGorselleri/" + newimagename;
            }
            else
            {
                values.EgitimKapakFotografi = values.EgitimKapakFotografi;
            }

            values.Kontenjan = model.Kontenjan;
            values.EgitimAdi = model.EgitimAdi;
            values.Suresi = model.Suresi;
            if (model.MentorId != null)
            {
                values.MentorId = model.MentorId;
            }
            else
            {
                values.MentorId = values.MentorId;
            }
            if (model.BaslangicTarihi != null)
            {
                values.BaslangicTarihi = model.BaslangicTarihi;
                values.BitisTarihi = model.BitisTarihi;
            }
            else
            {
                values.BaslangicTarihi = values.BaslangicTarihi;
                values.BitisTarihi = values.BitisTarihi;
            }

            _mentoregitim.TUpdate(values);
            string logMessage = "Mentor eğitimi düzenlendi: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
            return RedirectToAction("Index");
        }

        #endregion

        #region Sil
        public IActionResult Sil(int id)
        {
            var values = _mentoregitim.TGetById(id);
            _mentoregitim.TDelete(values);
            return RedirectToAction("Index");
        }
        #endregion

        #region MentorAtama
        [HttpPost]
        public IActionResult MentorAtama(int MentorId, int id)
        {
            var values = _mentoregitim.TGetById(id);
            values.MentorId = MentorId;
            _mentoregitim.TUpdate(values);
            return Json(values);
        }

        #endregion

        #region EgitimeKatil
        public IActionResult EgitimeKatil(int MentorEgitimId)
        {
            var userName = _http.HttpContext.Session.GetString("UserName");
            var userId = _http.HttpContext.Session.GetString("UserId");

            var egitim = _mentoregitim.TGetById(MentorEgitimId);
            var values = _kullanicivementormanager.TGetList(x => x.MentorEgitimleriId == MentorEgitimId).Count();
            var kullaniciEgitimKontrol = _kullanicivementormanager.TGetList(x => x.AppUserId == int.Parse(userId) && x.MentorEgitimleriId == MentorEgitimId).Count();

            if (values <= egitim.Kontenjan)
            {
                if (kullaniciEgitimKontrol == 0)
                {
                    KullaniciveMentorEgitimleri kullaniciveMentorEgitimleri = new KullaniciveMentorEgitimleri();
                    kullaniciveMentorEgitimleri.AppUserId = int.Parse(userId);
                    kullaniciveMentorEgitimleri.MentorEgitimleriId = MentorEgitimId;
                    _kullanicivementormanager.TAdd(kullaniciveMentorEgitimleri);
                    string logMessage = "Kullanıcı eğitime katıldı " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
                    string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

                    using (StreamWriter sw = new StreamWriter(logFilePath, true))
                    {
                        sw.WriteLine(logMessage);
                    }
                    var mentorEgitimiBul = _mentoregitim.TGetById(MentorEgitimId);
                    mentorEgitimiBul.Kontenjan = mentorEgitimiBul.Kontenjan = 1;
                    _mentoregitim.TUpdate(mentorEgitimiBul);
                    return RedirectToAction("Index");
                }
                else
                {
                    return Json(new { uyari = false });
                }
            }
            else
            {
                return Json(new { message = true });
            }



        }

        #endregion

        #region Katilimcilar
        public IActionResult Katilimcilar(int id)
        {
            var values = _kullanicivementormanager.TGetList(x => x.MentorEgitimleriId == id).Include(x => x.AppUser)
                .Select(x => new KatilimcilarModel
                {
                    Adi = x.AppUser.Isim,
                    Soyadi = x.AppUser.Soyisim,
                    Email = x.AppUser.UserName
                });
            return Json(values);

        }
        #endregion

        #region EgitimiDuzenle

        public IActionResult AlinanEgitimiDuzenle(int id, int mentorEgitimId )
        {
            var values = _kullanicivementormanager.TGetById(id);
            var kontenjanKontrol = _mentoregitim.TGetById(mentorEgitimId);           
            if (kontenjanKontrol.Kontenjan > 0)
            {
                if (values.MentorEgitimleriId != mentorEgitimId)
                {
                    int eskiMentorEgitimId = values.MentorEgitimleriId;
                    var eskiEgitimKontenjanGuncelle = _mentoregitim.TGetById(eskiMentorEgitimId);
                    eskiEgitimKontenjanGuncelle.Kontenjan = eskiEgitimKontenjanGuncelle.Kontenjan + 1;
                    _mentoregitim.TUpdate(eskiEgitimKontenjanGuncelle);
                }
                if (values.MentorEgitimleriId != mentorEgitimId)
                {
                    kontenjanKontrol.Kontenjan = kontenjanKontrol.Kontenjan - 1;
                    _mentoregitim.TUpdate(kontenjanKontrol);
                }
                values.MentorEgitimleriId = mentorEgitimId;
                _kullanicivementormanager.TUpdate(values); 
                
                return Json(new { message = true });
            }           
            else
            {
                return Json(new { message = false });
            }

          //  return RedirectToAction("Index");
        }
        #endregion

        #region EgitimiSil
        public IActionResult AlinanEgitimiSil(int id)
        {
            var values = _kullanicivementormanager.TGetById(id);
            _kullanicivementormanager.TDelete(values);
            return RedirectToAction("Index");
        } 
        #endregion

    }
}
