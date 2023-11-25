using BusinessLayer.Concrete;
using ClosedXML.Excel;
using DataAccess.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using ProtaWebPortal.Models;
using X.PagedList;

namespace ProtaWebPortal.Controllers
{
    public class EgitimController : Controller
    {
        private readonly EgitimManager _egitim;
        private readonly IHttpContextAccessor _http;
        private readonly CihazManager _cihazManager;
        private readonly ProtaDbContext _c;
        public EgitimController(EgitimManager egitimManager, IHttpContextAccessor http, CihazManager cihazManager, ProtaDbContext c)
        {
            _egitim = egitimManager;
            _http = http;
            _cihazManager = cihazManager;
            _c = c;
        }
        #region Index
        public IActionResult Index(int page = 1, string kullaniciAdi = "", string cihazAdi = "", bool tumumuGor = false, bool YeniEgitimIstekleri = false, bool devamedenEgitimler = false, bool onaylananEgitimler = false)
        {
            string userId = _http.HttpContext.Session.GetString("UserId");


            if (User.IsInRole("Admin"))
            {
                if (!string.IsNullOrEmpty(kullaniciAdi))
                {
                    var values = _egitim.TGetList().Include(x => x.cihaz).Include(x => x.appUser)
                   .Select(x => new EgitimIstekleriListe
                   {
                       Id = x.Id,
                       CihazAdi = x.cihaz.CihazAdi,
                       KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                       EgitimDurumu = x.EgitimDurumu,
                       EgitimNeDurumda = x.EgitimNeDurumda
                   })
                   .OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0) // İstek gönderildi olanları en üstte göstermek için sıralama
                   .ToPagedList(page, 10);

                    var lowercaseKullaniciAdi = kullaniciAdi.ToLower();
                    values = values.Where(x => x.KullaniciAdi.Contains(kullaniciAdi)).ToPagedList(page, 50);
                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }

                else if (!string.IsNullOrEmpty(cihazAdi))
                {
                    var values = _egitim.TGetList().Include(x => x.cihaz).Include(x => x.appUser)
                                      .Select(x => new EgitimIstekleriListe
                                      {
                                          Id = x.Id,
                                          CihazAdi = x.cihaz.CihazAdi,
                                          KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                                          EgitimDurumu = x.EgitimDurumu,
                                          EgitimNeDurumda = x.EgitimNeDurumda
                                      })
                                      .OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0) // İstek gönderildi olanları en üstte göstermek için sıralama
                                      .ToPagedList(page, 10);


                    values = values.Where(x => x.CihazAdi.Contains(cihazAdi)).ToPagedList(page, 50);
                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }

                else if (YeniEgitimIstekleri)
                {
                    var values = _egitim.TGetList().Where(x=>x.EgitimNeDurumda == "İstek gönderildi").Include(x => x.cihaz).Include(x => x.appUser)
                 .Select(x => new EgitimIstekleriListe
                 {
                     Id = x.Id,
                     CihazAdi = x.cihaz.CihazAdi,
                     KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                     EgitimDurumu = x.EgitimDurumu,
                     EgitimNeDurumda = x.EgitimNeDurumda
                 })                 
                 .ToPagedList(page, 10);

                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }

                else if (devamedenEgitimler)
                {
                    var values = _egitim.TGetList().Where(x=>x.EgitimNeDurumda == "Eğitim Başladı").Include(x => x.cihaz).Include(x => x.appUser)
             .Select(x => new EgitimIstekleriListe
             {
                 Id = x.Id,
                 CihazAdi = x.cihaz.CihazAdi,
                 KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                 EgitimDurumu = x.EgitimDurumu,
                 EgitimNeDurumda = x.EgitimNeDurumda
             })
            
             .ToPagedList(page, 10);

                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }

                else if(onaylananEgitimler)
                {
                    var values = _egitim.TGetList().Where(x=>x.EgitimNeDurumda == "Eğitim Onaylandı").Include(x => x.cihaz).Include(x => x.appUser)
            .Select(x => new EgitimIstekleriListe
            {
                Id = x.Id,
                CihazAdi = x.cihaz.CihazAdi,
                KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                EgitimDurumu = x.EgitimDurumu,
                EgitimNeDurumda = x.EgitimNeDurumda
            })           
            .ToPagedList(page, 10);

                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }

                else if (tumumuGor)
                {
                    var values = _egitim.TGetList().Include(x => x.cihaz).Include(x => x.appUser)
                  .Select(x => new EgitimIstekleriListe
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                      EgitimDurumu = x.EgitimDurumu,
                      EgitimNeDurumda = x.EgitimNeDurumda
                  })
                  .OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0) // İstek gönderildi olanları en üstte göstermek için sıralama
                  .ToPagedList(page, 10);

                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }

                else
                {
                    var values = _egitim.TGetList().Include(x => x.cihaz).Include(x => x.appUser)
                 .Select(x => new EgitimIstekleriListe
                 {
                     Id = x.Id,
                     CihazAdi = x.cihaz.CihazAdi,
                     KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                     EgitimDurumu = x.EgitimDurumu,
                     EgitimNeDurumda = x.EgitimNeDurumda
                 })
                 .OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0) // İstek gönderildi olanları en üstte göstermek için sıralama
                 .ToPagedList(page, 10);

                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }

            }

            else
            {
                if (!string.IsNullOrEmpty(kullaniciAdi))
                {
                    var valuess = _egitim.TGetList(x=>x.appUserId == int.Parse(userId)).Include(x => x.cihaz).Include(x => x.appUser)
                  .Select(x => new EgitimIstekleriListe
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                      EgitimDurumu = x.EgitimDurumu,
                      EgitimNeDurumda = x.EgitimNeDurumda
                  })
                  .OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0) // İstek gönderildi olanları en üstte göstermek için sıralama
                  .ToPagedList(page, 10);

                    var lowercaseKullaniciAdi = kullaniciAdi.ToLower();
                    valuess = valuess.Where(x => x.KullaniciAdi.Contains(kullaniciAdi)).ToPagedList(page, 50);
                    var cihazlarr = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var modell = (valuess, cihazlarr);
                    return View(modell);
                }

                else if (!string.IsNullOrEmpty(cihazAdi))
                {
                    var valuess = _egitim.TGetList(x=>x.appUserId == int.Parse(userId)).Include(x => x.cihaz).Include(x => x.appUser)
                                     .Select(x => new EgitimIstekleriListe
                                     {
                                         Id = x.Id,
                                         CihazAdi = x.cihaz.CihazAdi,
                                         KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                                         EgitimDurumu = x.EgitimDurumu,
                                         EgitimNeDurumda = x.EgitimNeDurumda
                                     })
                                     .OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0) // İstek gönderildi olanları en üstte göstermek için sıralama
                                     .ToPagedList(page, 10);


                    valuess = valuess.Where(x => x.CihazAdi.Contains(cihazAdi)).ToPagedList(page, 50);
                    var cihazlarr = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var modell = (valuess, cihazlarr);
                    return View(modell);
                }

                else if (YeniEgitimIstekleri)
                {
                    var valuess = _egitim.TGetList(x=>x.appUserId == int.Parse(userId)).Where(x => x.EgitimNeDurumda == "İstek gönderildi").Include(x => x.cihaz).Include(x => x.appUser)
                 .Select(x => new EgitimIstekleriListe
                 {
                     Id = x.Id,
                     CihazAdi = x.cihaz.CihazAdi,
                     KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                     EgitimDurumu = x.EgitimDurumu,
                     EgitimNeDurumda = x.EgitimNeDurumda
                 })
                 .ToPagedList(page, 10);

                    var cihazlarr = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var modell = (valuess, cihazlarr);
                    return View(modell);
                }

                else if (devamedenEgitimler)
                {
                    var valuess = _egitim.TGetList(x=>x.appUserId == int.Parse(userId)).Where(x => x.EgitimNeDurumda == "Eğitim Başladı").Include(x => x.cihaz).Include(x => x.appUser)
             .Select(x => new EgitimIstekleriListe
             {
                 Id = x.Id,
                 CihazAdi = x.cihaz.CihazAdi,
                 KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                 EgitimDurumu = x.EgitimDurumu,
                 EgitimNeDurumda = x.EgitimNeDurumda
             })

             .ToPagedList(page, 10);

                    var cihazlarr = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var modell = (valuess, cihazlarr);
                    return View(modell);
                }

                else if (onaylananEgitimler)
                {
                    var valuess = _egitim.TGetList(x=>x.appUserId == int.Parse(userId)).Where(x => x.EgitimNeDurumda == "Eğitim Onaylandı").Include(x => x.cihaz).Include(x => x.appUser)
            .Select(x => new EgitimIstekleriListe
            {
                Id = x.Id,
                CihazAdi = x.cihaz.CihazAdi,
                KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                EgitimDurumu = x.EgitimDurumu,
                EgitimNeDurumda = x.EgitimNeDurumda
            })
            .ToPagedList(page, 10);

                    var cihazlarr = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var modell = (valuess, cihazlarr);
                    return View(modell);
                }

                else if (tumumuGor)
                {
                    var valuess = _egitim.TGetList(x=>x.appUserId == int.Parse(userId)).Include(x => x.cihaz).Include(x => x.appUser)
                  .Select(x => new EgitimIstekleriListe
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
                      EgitimDurumu = x.EgitimDurumu,
                      EgitimNeDurumda = x.EgitimNeDurumda
                  })
                  .OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0) // İstek gönderildi olanları en üstte göstermek için sıralama
                  .ToPagedList(page, 10);

                    var cihazlarr = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var modell = (valuess, cihazlarr);
                    return View(modell);
                }
               
                else
                {
                    var values = _egitim.TGetList(x => x.appUserId == int.Parse(userId)).Include(x => x.cihaz).Include(x => x.appUser)
           .Select(x => new EgitimIstekleriListe
           {
               Id = x.Id,
               CihazAdi = x.cihaz.CihazAdi,
               KullaniciAdi = x.appUser.Isim + " " + x.appUser.Soyisim,
               EgitimDurumu = x.EgitimDurumu,
               EgitimNeDurumda = x.EgitimNeDurumda
           }).OrderByDescending(x => x.EgitimNeDurumda == "İstek gönderildi" ? 1 : 0).ToPagedList(page, 10);
                    var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true).ToPagedList(page, 50);
                    var model = (values, cihazlar);
                    return View(model);
                }             
            }
        }

        #endregion


        #region KullaniciListe
        public IActionResult KullaniciListe()
        {
            var values = _c.Users.ToList()
                 .Select(x => new UyeListeModel
                 {
                     Id = x.Id,
                     Adi = x.Isim,
                     Soyadi = x.Soyisim,
                     AdiSoyadi = x.Isim + " " + x.Soyisim
                 }).ToList();
            return Json(values);
        } 
        #endregion

        #region Ekle
        [HttpGet]
        public IActionResult EgitimIstegiEkle()
        {
            return View();
        }
        [HttpPost]
        public IActionResult EgitimIstegiEkle(Egitimler egitim)
        {          
           
            string userId = _http.HttpContext.Session.GetString("UserId");
            string userName = _http.HttpContext.Session.GetString("UserName");

            bool egitimAlindimi = _egitim.TGetList(x => x.appUserId == int.Parse(userId))
                                .Any(x => x.EgitimDurumu == true && x.cihazId == egitim.cihazId);
            if (egitimAlindimi == false)
            {
                Egitimler e = new Egitimler();
                e.cihazId = egitim.cihazId;
                e.appUserId = int.Parse(userId);
                e.EgitimDurumu = false;
                e.EgitimNeDurumda = "İstek gönderildi";
                _egitim.TAdd(e);
                string logMessage = "Egitim isteği gönderildi: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
                string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine(logMessage);
                }
                return Json(new {status = false});
            }
            else
            {
                return Json(new { status = true });
            }
           

        }

   
        #endregion

        #region EgitimiBaslat
        public IActionResult EgitimiBaslat(int id)
        {
            string userName = _http.HttpContext.Session.GetString("UserName");
            var values = _egitim.TGetById(id);
            values.EgitimNeDurumda = "Eğitim Başladı";
            _egitim.TUpdate(values);

            string logMessage = "Egitim başladı: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EgitimiOnayla
        public IActionResult EgitimiOnayla(int id)
        {
            string userName = _http.HttpContext.Session.GetString("UserName");
            var values = _egitim.TGetById(id);
            values.EgitimNeDurumda = "Eğitim Onaylandı";
            values.EgitimDurumu = true;
            _egitim.TUpdate(values);

            string logMessage = "Egitim Onaylandı: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EgitimiSil
        public IActionResult Sil(int id)
        {
            string userName = _http.HttpContext.Session.GetString("UserName");
            var values = _egitim.TGetById(id);
            _egitim.TDelete(values);
            string logMessage = "Egitim silindi: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Duzenle
        public IActionResult Duzenle(int id, int cihazId)
        {
            var values = _egitim.TGetById(id);
            values.cihazId = cihazId;
            _egitim.TUpdate(values);
            return RedirectToAction("Index");
        } 
        #endregion

    }
}
