using BusinessLayer.Concrete;
using ClosedXML.Excel;
using DataAccess.Concrete;
using DataAccess.Migrations;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtaWebPortal.Models;
using SelectPdf;
using System;
using System.Text;
using System.Transactions;
using X.PagedList;

namespace ProtaWebPortal.Controllers
{
    public class RezervasyonlarController : Controller
    {

        private readonly IHttpContextAccessor _http;
        private readonly CihazManager _cihazManager;
        private readonly RezervasyonManager _rezervasyonManager;
        private readonly FiyatlandirmaManager _fiyatlandirmaManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ProtaDbContext _c;
        private readonly EgitimManager _egitim;

        public RezervasyonlarController(IHttpContextAccessor http, RezervasyonManager rezervasyon, CihazManager cihazManager, FiyatlandirmaManager fiyatlandirmaManager, UserManager<AppUser> userManager, ProtaDbContext c,EgitimManager egitimManager)
        {
            _http = http;
            _rezervasyonManager = rezervasyon;
            _cihazManager = cihazManager;
            _fiyatlandirmaManager = fiyatlandirmaManager;
            _userManager = userManager;
            _c = c;
            _egitim = egitimManager;
        }

        #region Index
        public IActionResult Index(int page = 1, string searchTerm = "", string KullaniciAdi = "", string baslangicTarihi = "", string bitisTarihi = "", bool showAll = false, bool gecmisRezervasyonlar = false, bool grupRezervasyonlar = false, bool Gunluk = false, bool Haftalik = false, bool Aylik = false)
        {
            DateTime today = DateTime.Today;
            string? userid = _http.HttpContext.Session.GetString("UserId");
            string? name = _http.HttpContext.Session.GetString("AdSoyad");
            if (User.IsInRole("Admin"))
            {

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                        .Select(x => new RezervasyonListeModel
                        {
                            Id = x.Id,
                            CihazAdi = x.cihaz.CihazAdi,
                            BaslangicTarihi = x.BaslangicTarihi,
                            BitisTarihi = x.BitisTarihi,
                            KullaniciAdi = x.UserName,
                            Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                            Guid = x.RezervasyonGuid,
                            RezervasyonDurumu = x.RezervasyonDurumu
                        }).ToPagedList(page, 10);

                    var lowercaseSearchTerm = searchTerm.ToLower();
                    models = models.Where(c => c.CihazAdi.ToLower().Contains(lowercaseSearchTerm)).ToPagedList(page, 10);

                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);



                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       }).Where(c => c.RezervasyonBaslangicTarihi.Date >= today);

                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (!string.IsNullOrEmpty(KullaniciAdi))
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                .Select(x => new RezervasyonListeModel
                {
                    Id = x.Id,
                    CihazAdi = x.cihaz.CihazAdi,
                    BaslangicTarihi = x.BaslangicTarihi,
                    BitisTarihi = x.BitisTarihi,
                    KullaniciAdi = x.UserName,
                    Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                    Guid = x.RezervasyonGuid,
                    RezervasyonDurumu = x.RezervasyonDurumu
                }).ToPagedList(page, 10);

                    var lowercaseSearchTerm = baslangicTarihi;
                    models = models.Where(c => c.KullaniciAdi.Contains(KullaniciAdi)).ToPagedList(page, 10);
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       });
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (Gunluk)
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true)
                       .Include(x => x.Fiyatlandirma)
                       .Where(c => c.RezervasyonTuru == "Gunluk") // Sadece haftalık rezervasyonları filtrele
                       .Select(x => new RezervasyonListeModel
                       {
                           Id = x.Id,
                           CihazAdi = x.cihaz.CihazAdi,
                           BaslangicTarihi = x.BaslangicTarihi,
                           BitisTarihi = x.BitisTarihi,
                           KullaniciAdi = x.UserName,
                           Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                           Guid = x.RezervasyonGuid,
                           RezervasyonTuru = x.RezervasyonTuru,
                           RezervasyonDurumu = x.RezervasyonDurumu
                       })
                       .ToList();

                    var groupedModels = models.GroupBy(c => c.RezervasyonTuru) // Haftalık rezervasyonları grupla
                        .SelectMany(g => g.ToList())
                        .ToPagedList(page, 10);

                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                        .Select(x => new RezervasyonDetaylari
                        {
                            RezervasyonId = x.rezervasyonId,
                            KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                            RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                            RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                            ToplamFiyat = x.ToplamFiyat,
                            IndirimTutari = x.appUser.IndirimYuzdesi,
                            IndirimliFiyat = x.IndirimliFiyat,
                            CihazAdi = x.cihaz.CihazAdi,
                            CihazGorseli = x.cihaz.CihazGorseli,
                            CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                        });

                    var fiyatRezervasyonModel = (groupedModels, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (Haftalik)
                {

                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true)
                        .Include(x => x.Fiyatlandirma)
                        .Where(c => c.RezervasyonTuru == "Haftalik") // Sadece haftalık rezervasyonları filtrele
                        .Select(x => new RezervasyonListeModel
                        {
                            Id = x.Id,
                            CihazAdi = x.cihaz.CihazAdi,
                            BaslangicTarihi = x.BaslangicTarihi,
                            BitisTarihi = x.BitisTarihi,
                            KullaniciAdi = x.UserName,
                            Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                            Guid = x.RezervasyonGuid,
                            RezervasyonTuru = x.RezervasyonTuru,
                            RezervasyonDurumu = x.RezervasyonDurumu
                        })
                        .ToList();

                    var groupedModels = models.GroupBy(c => c.RezervasyonTuru) // Haftalık rezervasyonları grupla
                        .SelectMany(g => g.ToList())
                        .ToPagedList(page, 10);

                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                        .Select(x => new RezervasyonDetaylari
                        {
                            RezervasyonId = x.rezervasyonId,
                            KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                            RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                            RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                            ToplamFiyat = x.ToplamFiyat,
                            IndirimTutari = x.appUser.IndirimYuzdesi,
                            IndirimliFiyat = x.IndirimliFiyat,
                            CihazAdi = x.cihaz.CihazAdi,
                            CihazGorseli = x.cihaz.CihazGorseli,
                            CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                        });

                    var fiyatRezervasyonModel = (groupedModels, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);


                }

                else if (Aylik)
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true)
                     .Include(x => x.Fiyatlandirma)
                     .Where(c => c.RezervasyonTuru == "Aylik") // Sadece haftalık rezervasyonları filtrele
                     .Select(x => new RezervasyonListeModel
                     {
                         Id = x.Id,
                         CihazAdi = x.cihaz.CihazAdi,
                         BaslangicTarihi = x.BaslangicTarihi,
                         BitisTarihi = x.BitisTarihi,
                         KullaniciAdi = x.UserName,
                         Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                         Guid = x.RezervasyonGuid,
                         RezervasyonTuru = x.RezervasyonTuru,
                         RezervasyonDurumu = x.RezervasyonDurumu
                     })
                     .ToList();

                    var groupedModels = models.GroupBy(c => c.RezervasyonTuru) // Haftalık rezervasyonları grupla
                        .SelectMany(g => g.ToList())
                        .ToPagedList(page, 10);

                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                        .Select(x => new RezervasyonDetaylari
                        {
                            RezervasyonId = x.rezervasyonId,
                            KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                            RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                            RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                            ToplamFiyat = x.ToplamFiyat,
                            IndirimTutari = x.appUser.IndirimYuzdesi,
                            IndirimliFiyat = x.IndirimliFiyat,
                            CihazAdi = x.cihaz.CihazAdi,
                            CihazGorseli = x.cihaz.CihazGorseli,
                            CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                        });

                    var fiyatRezervasyonModel = (groupedModels, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (grupRezervasyonlar)
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                    .Select(x => new RezervasyonListeModel
                    {
                        Id = x.Id,
                        CihazAdi = x.cihaz.CihazAdi,
                        BaslangicTarihi = x.BaslangicTarihi,
                        BitisTarihi = x.BitisTarihi,
                        KullaniciAdi = x.UserName,
                        Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                        Guid = x.RezervasyonGuid,
                        RezervasyonDurumu = x.RezervasyonDurumu
                    }).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       });
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);

                    var duplicateGuids = models.GroupBy(c => c.Guid)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key);

                    models = models.Where(c => duplicateGuids.Contains(c.Guid)).ToPagedList(page, 10);

                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (showAll)
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                    .Select(x => new RezervasyonListeModel
                    {
                        Id = x.Id,
                        CihazAdi = x.cihaz.CihazAdi,
                        BaslangicTarihi = x.BaslangicTarihi,
                        BitisTarihi = x.BitisTarihi,
                        KullaniciAdi = x.UserName,
                        Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                        Guid = x.RezervasyonGuid,
                        RezervasyonDurumu = x.RezervasyonDurumu
                    }).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       });
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (gecmisRezervasyonlar)
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                    .Select(x => new RezervasyonListeModel
                    {
                        Id = x.Id,
                        CihazAdi = x.cihaz.CihazAdi,
                        BaslangicTarihi = x.BaslangicTarihi,
                        BitisTarihi = x.BitisTarihi,
                        KullaniciAdi = x.UserName,
                        Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                        Guid = x.RezervasyonGuid,
                        RezervasyonDurumu = x.RezervasyonDurumu
                    }).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       });
                    models = models.Where(c => c.BaslangicTarihi.Date < today).ToPagedList(page, 10);
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (!string.IsNullOrEmpty(baslangicTarihi))
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                  .Select(x => new RezervasyonListeModel
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      BaslangicTarihi = x.BaslangicTarihi,
                      BitisTarihi = x.BitisTarihi,
                      KullaniciAdi = x.UserName,
                      Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                      Guid = x.RezervasyonGuid,
                      RezervasyonDurumu = x.RezervasyonDurumu
                  }).ToPagedList(page, 10);

                    var lowercaseSearchTerm = baslangicTarihi;
                    models = models.Where(c => c.BaslangicTarihi.ToString("yyyy-MM-dd HH:mm").Contains(lowercaseSearchTerm)).ToPagedList(page, 10);
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       });
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (!string.IsNullOrEmpty(bitisTarihi))
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                  .Select(x => new RezervasyonListeModel
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      BaslangicTarihi = x.BaslangicTarihi,
                      BitisTarihi = x.BitisTarihi,
                      KullaniciAdi = x.UserName,
                      Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                      Guid = x.RezervasyonGuid,
                      RezervasyonDurumu = x.RezervasyonDurumu
                  }).ToPagedList(page, 10);

                    var lowercaseSearchTerm = bitisTarihi;
                    models = models.Where(c => c.BitisTarihi.ToString("yyyy-MM-dd HH:mm").Contains(lowercaseSearchTerm)).ToPagedList(page, 10);
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {
                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else
                {
                    var models = _rezervasyonManager.TGetList(x => x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                   .Select(x => new RezervasyonListeModel
                   {
                       Id = x.Id,
                       CihazAdi = x.cihaz.CihazAdi,
                       BaslangicTarihi = x.BaslangicTarihi,
                       BitisTarihi = x.BitisTarihi,
                       KullaniciAdi = x.UserName,
                       Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                       Guid = x.RezervasyonGuid,
                       RezervasyonDurumu = x.RezervasyonDurumu
                   }).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {
                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }
            }

            else
            {
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                   .Select(x => new RezervasyonListeModel
                   {
                       Id = x.Id,
                       CihazAdi = x.cihaz.CihazAdi,
                       BaslangicTarihi = x.BaslangicTarihi,
                       BitisTarihi = x.BitisTarihi,
                       KullaniciAdi = x.UserName,
                       Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                       Guid = x.RezervasyonGuid,
                       RezervasyonDurumu = x.RezervasyonDurumu
                   }).ToPagedList(page, 10);
                    var lowercaseSearchTerm = searchTerm.ToLower();
                    models = models.Where(c => c.CihazAdi.ToLower().Contains(lowercaseSearchTerm)).ToPagedList(page, 10);
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {
                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (!string.IsNullOrEmpty(KullaniciAdi))
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                .Select(x => new RezervasyonListeModel
                {
                    Id = x.Id,
                    CihazAdi = x.cihaz.CihazAdi,
                    BaslangicTarihi = x.BaslangicTarihi,
                    BitisTarihi = x.BitisTarihi,
                    KullaniciAdi = x.UserName,
                    Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                    Guid = x.RezervasyonGuid,
                    RezervasyonDurumu = x.RezervasyonDurumu
                }).ToPagedList(page, 10);

                    var lowercaseSearchTerm = baslangicTarihi;
                    models = models.Where(c => c.KullaniciAdi.Contains(KullaniciAdi)).ToPagedList(page, 10);
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       });
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (showAll)
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                  .Select(x => new RezervasyonListeModel
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      BaslangicTarihi = x.BaslangicTarihi,
                      BitisTarihi = x.BitisTarihi,
                      KullaniciAdi = x.UserName,
                      Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                      Guid = x.RezervasyonGuid,
                      RezervasyonDurumu = x.RezervasyonDurumu
                  }).ToPagedList(page, 10);

                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {
                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (grupRezervasyonlar)
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                    .Select(x => new RezervasyonListeModel
                    {
                        Id = x.Id,
                        CihazAdi = x.cihaz.CihazAdi,
                        BaslangicTarihi = x.BaslangicTarihi,
                        BitisTarihi = x.BitisTarihi,
                        KullaniciAdi = x.UserName,
                        Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                        Guid = x.RezervasyonGuid,
                        RezervasyonDurumu = x.RezervasyonDurumu
                    }).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                       .Select(x => new RezervasyonDetaylari
                       {
                           RezervasyonId = x.rezervasyonId,
                           KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                           RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                           RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                           ToplamFiyat = x.ToplamFiyat,
                           IndirimTutari = x.appUser.IndirimYuzdesi,
                           IndirimliFiyat = x.IndirimliFiyat,
                           CihazAdi = x.cihaz.CihazAdi,
                           CihazGorseli = x.cihaz.CihazGorseli,
                           CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                       });
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);

                    var duplicateGuids = models.GroupBy(c => c.Guid)
                                       .Where(g => g.Count() > 1)
                                       .Select(g => g.Key);

                    models = models.Where(c => duplicateGuids.Contains(c.Guid)).ToPagedList(page, 10);

                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }


                else if (gecmisRezervasyonlar)
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                 .Select(x => new RezervasyonListeModel
                 {
                     Id = x.Id,
                     CihazAdi = x.cihaz.CihazAdi,
                     BaslangicTarihi = x.BaslangicTarihi,
                     BitisTarihi = x.BitisTarihi,
                     KullaniciAdi = x.UserName,
                     Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                     Guid = x.RezervasyonGuid,
                     RezervasyonDurumu = x.RezervasyonDurumu
                 }).ToPagedList(page, 10);

                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {
                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    models = models.Where(c => c.BaslangicTarihi.Date < today).ToPagedList(page, 10);
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (!string.IsNullOrEmpty(baslangicTarihi))
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                  .Select(x => new RezervasyonListeModel
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      BaslangicTarihi = x.BaslangicTarihi,
                      BitisTarihi = x.BitisTarihi,
                      KullaniciAdi = x.UserName,
                      Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                      Guid = x.RezervasyonGuid,
                      RezervasyonDurumu = x.RezervasyonDurumu
                  }).ToPagedList(page, 10);

                    var lowercaseSearchTerm = baslangicTarihi;
                    models = models.Where(c => c.BaslangicTarihi.ToString("yyyy-MM-dd HH:mm").Contains(lowercaseSearchTerm)).ToPagedList(page, 10);
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {

                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else if (!string.IsNullOrEmpty(bitisTarihi))
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                  .Select(x => new RezervasyonListeModel
                  {
                      Id = x.Id,
                      CihazAdi = x.cihaz.CihazAdi,
                      BaslangicTarihi = x.BaslangicTarihi,
                      BitisTarihi = x.BitisTarihi,
                      KullaniciAdi = x.UserName,
                      Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                      Guid = x.RezervasyonGuid,
                      RezervasyonDurumu = x.RezervasyonDurumu
                  }).ToPagedList(page, 10);

                    var lowercaseSearchTerm = bitisTarihi;
                    models = models.Where(c => c.BitisTarihi.ToString("yyyy-MM-dd HH:mm").Contains(lowercaseSearchTerm)).ToPagedList(page, 10);
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {
                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

                else
                {
                    var models = _rezervasyonManager.TGetList(x => x.UserId == userid && x.RezervasyonDurumu == true).Include(x => x.Fiyatlandirma)
                   .Select(x => new RezervasyonListeModel
                   {
                       Id = x.Id,
                       CihazAdi = x.cihaz.CihazAdi,
                       BaslangicTarihi = x.BaslangicTarihi,
                       BitisTarihi = x.BitisTarihi,
                       KullaniciAdi = x.UserName,
                       Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                       Guid = x.RezervasyonGuid,
                       RezervasyonDurumu = x.RezervasyonDurumu
                   }).ToPagedList(page, 10);
                    var FiyatlandirmaListe = _fiyatlandirmaManager.TGetList()
                      .Select(x => new RezervasyonDetaylari
                      {
                          RezervasyonId = x.rezervasyonId,
                          KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                          RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                          RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                          ToplamFiyat = x.ToplamFiyat,
                          IndirimTutari = x.appUser.IndirimYuzdesi,
                          IndirimliFiyat = x.IndirimliFiyat,
                          CihazAdi = x.cihaz.CihazAdi,
                          CihazGorseli = x.cihaz.CihazGorseli,
                          CihazSaatlikFiyat = x.cihaz.SaatlikFiyat
                      });
                    models = models.Where(c => c.BaslangicTarihi.Date >= today).ToPagedList(page, 10);
                    var fiyatRezervasyonModel = (models, FiyatlandirmaListe);
                    return View(fiyatRezervasyonModel);
                }

            }
        }
        #endregion

        #region KullaniciListele
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
        #endregion

        #region CihazListele
        public IActionResult CihazListele()
        {
            var values = _cihazManager.TGetList(x => x.CihazDurum == true);
            return Json(values);
        }
        #endregion

        #region RezervasyonEkleme
        [HttpGet]
        public IActionResult RezervasyonEkle()
        {
            List<SelectListItem> value = (from u in _cihazManager.TGetList(x => x.CihazDurum == true)
                                          select new SelectListItem
                                          {
                                              Text = u.CihazAdi,
                                              Value = u.Id.ToString()
                                          }).ToList();
            ViewBag.v1 = value;

            var rezervasonekle = new RezervasyonEkleModel();
            var cihazlar = _cihazManager.TGetList(x => x.CihazDurum == true);
            var model = (rezervasonekle, cihazlar);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RezervasyonEkle(RezervasyonEkleModel rezervasyon)
        {
            List<int> fiyatlandirmaIds = new List<int>();
            string? userid = _http.HttpContext.Session.GetString("UserId");
            string? name = _http.HttpContext.Session.GetString("AdSoyad");
            string userName = _http.HttpContext.Session.GetString("UserName");
            int seçilenHaftaSayisi = Convert.ToInt32(rezervasyon.HaftalikRezervasyon);
            Guid rezervasyonGuid = Guid.NewGuid();
            for (int i = 0; i < seçilenHaftaSayisi; i++)
            {
                Rezervasyon _rezervasyon = new Rezervasyon();
                _rezervasyon.BaslangicTarihi = i == 0 ? rezervasyon.BaslangicTarihi : rezervasyon.BaslangicTarihi.AddDays(7 * i);
                _rezervasyon.BitisTarihi = i == 0 ? rezervasyon.BitisTarihi : rezervasyon.BitisTarihi.AddDays(7 * i);
                _rezervasyon.Aciklama = rezervasyon.Aciklama;
                _rezervasyon.RezervasyonDurumu = true;
                _rezervasyon.CihazId = int.Parse(rezervasyon.CihazAdi);
                _rezervasyon.UserId = userid;
                _rezervasyon.BildirimAl = false;
                _rezervasyon.RezervasyonTuru = rezervasyon.RezervasyonTuru;
                _rezervasyon.RezervasyonKayitTarihi = DateTime.Now;
                _rezervasyon.UserName = name;
                if (seçilenHaftaSayisi == 1)
                {
                    _rezervasyon.RezervasyonGuid = Guid.NewGuid().ToString(); // Yeni bir GUID oluştur
                }
                else
                {
                    _rezervasyon.RezervasyonGuid = rezervasyonGuid.ToString(); // Daha önce oluşturulan GUID'i kullan
                }

                _rezervasyonManager.TAdd(_rezervasyon);

                int fiyatid = await RezervasyonKullanımHesapla(userid, _rezervasyon);
                fiyatlandirmaIds.Add(fiyatid);

                var kullaniciBilgi = await _userManager.FindByIdAsync(_rezervasyon.UserId);

                Mail mail = new Mail();
                mail.MailGonder("Rezervasyon Bilgileri", kullaniciBilgi.UserName, kullaniciBilgi.Isim + " " + kullaniciBilgi.Soyisim, $"{_rezervasyon.BaslangicTarihi} {_rezervasyon.BitisTarihi} tarihleri arasında rezervasyonunuz oluşturuldu.");

            }


            string logMessage = "Yeni rezervasyon eklendi: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }

            return Json(new { id = fiyatlandirmaIds });

        }
        private async Task<int> RezervasyonKullanımHesapla(string userid, Rezervasyon rezervasyon)
        {
            var kullaniciIndirimKontrol = await _userManager.FindByIdAsync(userid);//Oturumdaki kullanıcıyı bul
            Fiyatlandirma fiyatlandirma = new Fiyatlandirma();
            if (kullaniciIndirimKontrol.IndirimYuzdesi != 0)
            {
                if (rezervasyon.BaslangicTarihi.Date == rezervasyon.BitisTarihi.Date)
                {
                    DateTime rezBaslangicSaati = rezervasyon.BaslangicTarihi; //Baslangic Saati al
                    DateTime rezBitisSaati = rezervasyon.BitisTarihi; // Bitiş saatini al
                    TimeSpan toplamSaat = rezBitisSaati - rezBaslangicSaati; // Kaç saat rezerve etmiş bul
                    var saat = toplamSaat.TotalHours;

                    var cihazBul = _cihazManager.TGetById(rezervasyon.CihazId); //Kaydedilen Id ye göre cihazı bul
                    var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                    var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)saat; // Cihazın rezervasyon saatine göre fiyatını bul

                    var indirimOranı = kullaniciIndirimKontrol.IndirimYuzdesi; // Kullanıcının indirimini al
                    var indirimliFiyat = cihazToplamRezervasyonFiyati * indirimOranı / 100; // indirim oranına göre fiyatı bul
                    var genelTutar = cihazToplamRezervasyonFiyati - indirimliFiyat;

                    //Fiyatlandırma tablosuna 

                    fiyatlandirma.appUserId = kullaniciIndirimKontrol.Id;
                    fiyatlandirma.IndirimliFiyat = indirimliFiyat;
                    fiyatlandirma.cihazId = cihazBul.Id;
                    fiyatlandirma.rezervasyonId = rezervasyon.Id;
                    fiyatlandirma.ToplamFiyat = genelTutar;
                    _fiyatlandirmaManager.TAdd(fiyatlandirma);
                }
                else
                {
                    DateTime rezBaslangicSaati = rezervasyon.BaslangicTarihi; //Baslangic Saati al
                    DateTime rezBitisSaati = rezervasyon.BitisTarihi; // Bitiş saatini al
                    TimeSpan toplamSaat = rezBitisSaati - rezBaslangicSaati; // Kaç saat rezerve etmiş bul                    

                    var toplamgun_saat = toplamSaat.TotalHours;

                    var cihazBul = _cihazManager.TGetById(rezervasyon.CihazId); //Kaydedilen Id ye göre cihazı bul
                    var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                    var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)toplamgun_saat; // Cihazın rezervasyon saatine göre fiyatını bul
                    var indirimOranı = kullaniciIndirimKontrol.IndirimYuzdesi; // Kullanıcının indirimini al
                    var indirimliFiyat = cihazToplamRezervasyonFiyati * indirimOranı / 100; // indirim oranına göre fiyatı bul
                    var genelTutar = cihazToplamRezervasyonFiyati - indirimliFiyat;

                    //Fiyatlandırma tablosuna 

                    fiyatlandirma.appUserId = kullaniciIndirimKontrol.Id;
                    fiyatlandirma.IndirimliFiyat = indirimliFiyat;
                    fiyatlandirma.cihazId = cihazBul.Id;
                    fiyatlandirma.rezervasyonId = rezervasyon.Id;
                    fiyatlandirma.ToplamFiyat = genelTutar;
                    _fiyatlandirmaManager.TAdd(fiyatlandirma);
                }

            }
            else
            {
                if (rezervasyon.BaslangicTarihi.Date == rezervasyon.BitisTarihi.Date)
                {
                    DateTime rezBaslangicSaati = rezervasyon.BaslangicTarihi; //Baslangic Saati al
                    DateTime rezBitisSaati = rezervasyon.BitisTarihi; // Bitiş saatini al
                    TimeSpan toplamSaat = rezBitisSaati - rezBaslangicSaati; // Kaç saat rezerve etmiş bul
                    var toplamGun_saat = toplamSaat.TotalHours;

                    var cihazBul = _cihazManager.TGetById(rezervasyon.CihazId); //Kaydedilen Id ye göre cihazı bul
                    var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                    var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)toplamGun_saat; // Cihazın rezervasyon saatine göre fiyatını bul

                    //Fiyatlandırma tablosuna 
                    fiyatlandirma.appUserId = kullaniciIndirimKontrol.Id;
                    fiyatlandirma.cihazId = cihazBul.Id;
                    fiyatlandirma.rezervasyonId = rezervasyon.Id;
                    fiyatlandirma.ToplamFiyat = cihazToplamRezervasyonFiyati;
                    _fiyatlandirmaManager.TAdd(fiyatlandirma);
                }
                else
                {
                    DateTime rezBaslangicSaati = rezervasyon.BaslangicTarihi; //Baslangic Saati al
                    DateTime rezBitisSaati = rezervasyon.BitisTarihi; // Bitiş saatini al
                    TimeSpan toplamSaat = rezBitisSaati - rezBaslangicSaati; // Kaç saat rezerve etmiş bul

                    var toplamGun_saat = toplamSaat.TotalHours;

                    var cihazBul = _cihazManager.TGetById(rezervasyon.CihazId); //Kaydedilen Id ye göre cihazı bul
                    var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                    var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)toplamGun_saat; // Cihazın rezervasyon saatine                    
                    //Fiyatlandırma tablosuna 

                    fiyatlandirma.appUserId = kullaniciIndirimKontrol.Id;

                    fiyatlandirma.cihazId = cihazBul.Id;
                    fiyatlandirma.rezervasyonId = rezervasyon.Id;
                    fiyatlandirma.ToplamFiyat = cihazToplamRezervasyonFiyati;
                    _fiyatlandirmaManager.TAdd(fiyatlandirma);
                }

            }
            return fiyatlandirma.Id;
        }
        #endregion

        #region RezervasyonKontrol
        [HttpGet]
        public IActionResult CihazRezervasyonKontrol(int cihaz)
        {
            var reservations = _rezervasyonManager.TGetList(r => r.CihazId == cihaz);

            List<Rezervasyon> reservationInfoList = new List<Rezervasyon>();
            foreach (var reservation in reservations)
            {
                var reservationInfo = new Rezervasyon
                {
                    Id = reservation.Id,
                    Aciklama = reservation.Aciklama,
                    CihazId = reservation.CihazId,
                    BaslangicTarihi = reservation.BaslangicTarihi,
                    BitisTarihi = reservation.BitisTarihi,
                    RezervasyonDurumu = reservation.RezervasyonDurumu,

                };

                reservationInfoList.Add(reservationInfo);
            }

            return Json(reservationInfoList);
        }
        #endregion

        #region Sil
        public IActionResult Sil(int id)
        {
            var values1 = _fiyatlandirmaManager.TGetList(x => x.rezervasyonId == id).FirstOrDefault();
            _fiyatlandirmaManager.TDelete(values1);
            var values = _rezervasyonManager.TGetById(id);
            _rezervasyonManager.TDelete(values);
            return Redirect("/Rezervasyonlar/Index");
        }
        #endregion

        #region Düzenle
        [HttpGet]
        public IActionResult Duzenle(int id)
        {
            var rezervasyon = _rezervasyonManager.TGetById(id);
            var fiyatBul = _fiyatlandirmaManager.TGetList(x => x.rezervasyonId == id).Include(x => x.cihaz).FirstOrDefault();
            ViewBag.cihazAdi = fiyatBul.cihaz.CihazAdi;
            ViewBag.fiyat = fiyatBul.ToplamFiyat;

            return View(rezervasyon);
        }

        [HttpPost]
        public async Task<IActionResult> Duzenle(Rezervasyon rezervasyon, Cihazlar cihazlar, int rezervasyonId)
        {
            Mail mail = new Mail();
            string username = _http.HttpContext.Session.GetString("UserName");
            var rezervasyonBul = _rezervasyonManager.TGetById(rezervasyonId);
            rezervasyonBul.UserName = rezervasyonBul.UserName;
            rezervasyonBul.RezervasyonKayitTarihi = rezervasyonBul.RezervasyonKayitTarihi;
            rezervasyonBul.UserId = rezervasyonBul.UserId;
            rezervasyonBul.RezervasyonDurumu = rezervasyonBul.RezervasyonDurumu;
            rezervasyonBul.BildirimAl = rezervasyonBul.BildirimAl;
            rezervasyonBul.CihazId = rezervasyon.CihazId;
            rezervasyonBul.Aciklama = rezervasyon.Aciklama;
            rezervasyonBul.BaslangicTarihi = rezervasyon.BaslangicTarihi;
            rezervasyonBul.BitisTarihi = rezervasyon.BitisTarihi;
            rezervasyonBul.RezervasyonGuid = rezervasyonBul.RezervasyonGuid;
            _rezervasyonManager.TUpdate(rezervasyonBul);
            string logMessage = "Rezervasyon güncellendi: " + "İşlemi yapan kullanıcı adı: " + " " + username + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }

                var kullaniciBilgi = await _userManager.FindByIdAsync(rezervasyonBul.UserId);
                mail.MailGonder("Rezervasyon bilgileriniz güncellendi.", kullaniciBilgi.UserName, kullaniciBilgi.Isim, $"{rezervasyonBul.BaslangicTarihi} {rezervasyonBul.BitisTarihi} tarihleri arasında rezervasyon bilgileriniz güncellendi.");
            
           

            //Fiyat güncelleme
            string userId = _http.HttpContext.Session.GetString("UserId");

            // var kullaniciIndirimKontrol = await _userManager.FindByIdAsync(userId);//Oturumdaki kullanıcıyı bul
            var fiyatiBul = _fiyatlandirmaManager.TGetList(x => x.rezervasyonId == rezervasyonId).Include(x => x.appUser).FirstOrDefault();
            if (fiyatiBul != null)
            {
                if (fiyatiBul.appUser.IndirimYuzdesi != 0)
                {

                    if (rezervasyonBul.BaslangicTarihi.Date == rezervasyonBul.BitisTarihi.Date)
                    {
                        DateTime rezervasyonBaslangicTarihi = rezervasyonBul.BaslangicTarihi;
                        DateTime rezervasyonBitisTarihi = rezervasyonBul.BitisTarihi;
                        TimeSpan toplamSaat = rezervasyonBitisTarihi - rezervasyonBaslangicTarihi;
                        var toplamgun_saat = toplamSaat.TotalHours;

                        var cihazBul = _cihazManager.TGetById(rezervasyonBul.CihazId); //Kaydedilen Id ye göre cihazı bul
                        var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                        var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)toplamgun_saat; // Cihazın rezervasyon saatine göre fiyatını bul

                        var indirimOranı = fiyatiBul.appUser.IndirimYuzdesi;  // Kullanıcının indirimini al
                        var indirimliFiyat = cihazToplamRezervasyonFiyati * indirimOranı / 100; // indirim oranına göre fiyatı bul
                        var genelTutar = cihazToplamRezervasyonFiyati - indirimliFiyat;

                        fiyatiBul.ToplamFiyat = genelTutar;
                        fiyatiBul.appUserId = fiyatiBul.appUserId;
                        fiyatiBul.IndirimliFiyat = indirimliFiyat;
                        fiyatiBul.cihazId = fiyatiBul.cihazId;
                        fiyatiBul.rezervasyonId = fiyatiBul.rezervasyonId;
                        _fiyatlandirmaManager.TUpdate(fiyatiBul);



                    }
                    #region eski
                    //var rezervasyonBaslangicTarihi = rezervasyonBul.BaslangicTarihi.Hour;
                    //var rezervasyonBitisTarihi = rezervasyonBul.BitisTarihi.Hour;
                    //var toplamSaat = rezervasyonBitisTarihi - rezervasyonBaslangicTarihi;

                    //var cihazBul = _cihazManager.TGetById(rezervasyonBul.CihazId); //Kaydedilen Id ye göre cihazı bul
                    //var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                    //var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * toplamSaat; // Cihazın rezervasyon saatine göre fiyatını bul
                    //var indirimOranı = fiyatiBul.appUser.IndirimYuzdesi;  // Kullanıcının indirimini al
                    //var indirimliFiyat = cihazToplamRezervasyonFiyati * indirimOranı / 100; // indirim oranına göre fiyatı bul
                    //var genelTutar = cihazToplamRezervasyonFiyati - indirimliFiyat;

                    //fiyatiBul.ToplamFiyat = genelTutar;
                    //fiyatiBul.appUserId = fiyatiBul.appUserId;
                    //fiyatiBul.IndirimliFiyat = indirimliFiyat;
                    //fiyatiBul.cihazId = fiyatiBul.cihazId;
                    //fiyatiBul.rezervasyonId = fiyatiBul.rezervasyonId;
                    //_fiyatlandirmaManager.TUpdate(fiyatiBul); 
                    #endregion


                    else
                    {
                        DateTime rezervasyonBaslangicTarihi = rezervasyonBul.BaslangicTarihi;
                        DateTime rezervasyonBitisTarihi = rezervasyonBul.BitisTarihi;
                        TimeSpan toplamSaat = rezervasyonBitisTarihi - rezervasyonBaslangicTarihi;
                        var toplamgun_saat = toplamSaat.TotalHours;

                        var cihazBul = _cihazManager.TGetById(rezervasyonBul.CihazId); //Kaydedilen Id ye göre cihazı bul
                        var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                        var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)toplamgun_saat; // Cihazın rezervasyon saatine göre fiyatını bul

                        var indirimOranı = fiyatiBul.appUser.IndirimYuzdesi;  // Kullanıcının indirimini al
                        var indirimliFiyat = cihazToplamRezervasyonFiyati * indirimOranı / 100; // indirim oranına göre fiyatı bul
                        var genelTutar = cihazToplamRezervasyonFiyati - indirimliFiyat;

                        fiyatiBul.ToplamFiyat = genelTutar;
                        fiyatiBul.appUserId = fiyatiBul.appUserId;
                        fiyatiBul.IndirimliFiyat = indirimliFiyat;
                        fiyatiBul.cihazId = fiyatiBul.cihazId;
                        fiyatiBul.rezervasyonId = fiyatiBul.rezervasyonId;
                        _fiyatlandirmaManager.TUpdate(fiyatiBul);
                    }

                }
                else
                {
                    if (rezervasyonBul.BaslangicTarihi.Date == rezervasyonBul.BitisTarihi.Date)
                    {
                        DateTime rezervasyonBaslangicTarihi = rezervasyonBul.BaslangicTarihi;
                        DateTime rezervasyonBitisTarihi = rezervasyonBul.BitisTarihi;
                        TimeSpan toplamsaat = rezervasyonBitisTarihi - rezervasyonBaslangicTarihi;
                        var toplamgun_saat = toplamsaat.TotalHours;

                        var cihazBul = _cihazManager.TGetById(rezervasyonBul.CihazId); //Kaydedilen Id ye göre cihazı bul
                        var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                        var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)toplamgun_saat; // Cihazın rezervasyon saatine göre fiyatını bul

                        fiyatiBul.ToplamFiyat = cihazToplamRezervasyonFiyati;
                        fiyatiBul.appUserId = fiyatiBul.appUserId;
                        fiyatiBul.cihazId = fiyatiBul.cihazId;
                        fiyatiBul.IndirimliFiyat = fiyatiBul.IndirimliFiyat;
                        fiyatiBul.rezervasyonId = fiyatiBul.rezervasyonId;
                        _fiyatlandirmaManager.TUpdate(fiyatiBul);

                    }
                    else
                    {
                        DateTime rezervasyonBaslangicTarihi = rezervasyonBul.BaslangicTarihi;
                        DateTime rezervasyonBitisTarihi = rezervasyonBul.BitisTarihi;
                        TimeSpan toplamsaat = rezervasyonBitisTarihi - rezervasyonBaslangicTarihi;
                        var toplamgun_saat = toplamsaat.TotalHours;

                        var cihazBul = _cihazManager.TGetById(rezervasyonBul.CihazId); //Kaydedilen Id ye göre cihazı bul
                        var cihazSaatlikFiyat = cihazBul.SaatlikFiyat; //Cihazın saatlik fiyatını al
                        var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * (decimal)toplamgun_saat; // Cihazın rezervasyon saatine göre fiyatını bul

                        fiyatiBul.ToplamFiyat = cihazToplamRezervasyonFiyati;
                        fiyatiBul.appUserId = fiyatiBul.appUserId;
                        fiyatiBul.cihazId = fiyatiBul.cihazId;
                        fiyatiBul.IndirimliFiyat = fiyatiBul.IndirimliFiyat;
                        fiyatiBul.rezervasyonId = fiyatiBul.rezervasyonId;
                        _fiyatlandirmaManager.TUpdate(fiyatiBul);
                    }

                }

            }



            return Redirect("/Rezervasyonlar/Index/");

        }

        #endregion

        #region RezervasyonGetir
        public IActionResult RezervasyonGetir(int id)
        {
            var reservations = _rezervasyonManager.TGetList(r => r.CihazId == id);

            List<Rezervasyon> reservationInfoList = new List<Rezervasyon>();
            foreach (var reservation in reservations)
            {
                var reservationInfo = new Rezervasyon
                {
                    Id = reservation.Id,
                    Aciklama = reservation.Aciklama,
                    CihazId = reservation.CihazId,
                    BaslangicTarihi = reservation.BaslangicTarihi,
                    BitisTarihi = reservation.BitisTarihi
                };

                reservationInfoList.Add(reservationInfo);
            }

            return Json(reservationInfoList);
        }
        #endregion

        #region RezervasyonDetayı
        [HttpGet]
        public IActionResult RezervasyonDetay()
        {
            List<int> ids = new List<int>();
            List<string> strlist = new List<string>(Request.RouteValues["id"].ToString().Split(','));

            for (int i = 0; i < strlist.Count; i++)
            {
                ids.Add(int.Parse(strlist[i]));
            }
            var values = _fiyatlandirmaManager.TGetList(x => ids.Contains(x.Id))
                .Select(x => new RezervasyonDetaylari
                {
                    KullaniciAdi = x.appUser.Isim + x.appUser.Soyisim,
                    RezervasyonBaslangicTarihi = x.rezervasyon.BaslangicTarihi,
                    RezervasyonBitisTarihi = x.rezervasyon.BitisTarihi,
                    ToplamFiyat = x.ToplamFiyat,
                    IndirimTutari = x.appUser.IndirimYuzdesi,
                    CihazAdi = x.cihaz.CihazAdi,
                    CihazGorseli = x.cihaz.CihazGorseli,
                    CihazSaatlikFiyat = x.cihaz.SaatlikFiyat,
                    IndirimliFiyat = x.IndirimliFiyat
                });


            return View(values);
        }
        #endregion


        public IActionResult Excel()
        {

            using (var excel = new XLWorkbook())
            {

                var models = _rezervasyonManager.TGetList().Include(x => x.Fiyatlandirma)
                       .Select(x => new RezervasyonListeModel
                       {
                           Id = x.Id,
                           CihazAdi = x.cihaz.CihazAdi,
                           BaslangicTarihi = x.BaslangicTarihi,
                           BitisTarihi = x.BitisTarihi,
                           KullaniciAdi = x.UserName,
                           Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                           Guid = x.RezervasyonGuid
                       });



                var worksheet = excel.Worksheets.Add("Rezervasyonlar");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Kullanıcı Adı";
                worksheet.Cell(currentRow, 2).Value = "Cihaz Adı";
                worksheet.Cell(currentRow, 3).Value = "Başlangıç Tarihi";
                worksheet.Cell(currentRow, 4).Value = "Bitiş Tarihi";
                worksheet.Cell(currentRow, 5).Value = "Toplam Tutar";

                foreach (var item in models)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.KullaniciAdi;
                    worksheet.Cell(currentRow, 2).Value = item.CihazAdi;
                    worksheet.Cell(currentRow, 3).Value = item.BaslangicTarihi;
                    worksheet.Cell(currentRow, 4).Value = item.BitisTarihi;
                    worksheet.Cell(currentRow, 5).Value = item.Fiyat + "₺";
                }

                using (var stream = new MemoryStream())
                {
                    excel.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "rezervasyonlar.xlsx");
                }
            }


        }


        public IActionResult Pdf()
        {
            var models = _rezervasyonManager.TGetList().Include(x => x.Fiyatlandirma)
                .Select(x => new RezervasyonListeModel
                {
                    Id = x.Id,
                    CihazAdi = x.cihaz.CihazAdi,
                    BaslangicTarihi = x.BaslangicTarihi,
                    BitisTarihi = x.BitisTarihi,
                    KullaniciAdi = x.UserName,
                    Fiyat = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                    Guid = x.RezervasyonGuid
                })
                .ToList();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Rezervasyonlar");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Kullanıcı Adı";
            worksheet.Cell(currentRow, 2).Value = "Cihaz Adı";
            worksheet.Cell(currentRow, 3).Value = "Başlangıç Tarihi";
            worksheet.Cell(currentRow, 4).Value = "Bitiş Tarihi";
            worksheet.Cell(currentRow, 5).Value = "Toplam Tutar";

            foreach (var item in models)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = item.KullaniciAdi;
                worksheet.Cell(currentRow, 2).Value = item.CihazAdi;
                worksheet.Cell(currentRow, 3).Value = item.BaslangicTarihi;
                worksheet.Cell(currentRow, 4).Value = item.BitisTarihi;
                worksheet.Cell(currentRow, 5).Value = item.Fiyat + "₺";
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Position = 0;

                var htmlToPdf = new SelectPdf.HtmlToPdf();
                htmlToPdf.Options.PdfPageSize = PdfPageSize.A4;
                htmlToPdf.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
                htmlToPdf.Options.WebPageWidth = 1024;
                htmlToPdf.Options.WebPageHeight = 0;

                var htmlContent = "<html><body><table style='border-collapse: collapse; width: 100%;'>" +
                "<thead><tr style='background-color: #f2f2f2;'>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Kullanıcı Adı</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Cihaz Adı</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Başlangıç Tarihi</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Bitiş Tarihi</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Toplam Tutar</th>" +
                "</tr></thead><tbody>";

                htmlContent += string.Join("", models.Select(item =>
                    "<tr>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.KullaniciAdi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.CihazAdi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.BaslangicTarihi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.BitisTarihi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Fiyat}₺</td>" +
                    "</tr>"
                ));

                htmlContent += "</tbody></table></body></html>";

                var pdfDocument = htmlToPdf.ConvertHtmlString(htmlContent);


                using (var memoryStream = new MemoryStream())
                {
                    pdfDocument.Save(memoryStream);
                    memoryStream.Position = 0;

                    return File(
                        memoryStream.ToArray(),
                        "application/pdf",
                        "rezervasyonlar.pdf");
                }
            }
        }


        public IActionResult CihazEgitimKontrol(int cihazId)
        {            
            bool egitimGerekli = _c.cihazlars.Any(c => c.Id == cihazId && c.EgitimGerekliMi);                                    
            string userId = _http.HttpContext.Session.GetString("UserId");
            bool egitimAlmis = _c.egitimlers.Any(e => e.appUserId == int.Parse(userId) && e.cihazId == cihazId && e.EgitimNeDurumda == "Eğitim Onaylandı");
            bool egitimDurumuIstekGonderildi = _c.egitimlers.Any(x => x.appUserId == int.Parse(userId) && x.cihazId == cihazId && x.EgitimNeDurumda == "İstek gönderildi");
            bool egitimbasladimi = _c.egitimlers.Any(x => x.appUserId == int.Parse(userId) && x.cihazId == cihazId && x.EgitimNeDurumda == "Eğitim Başladı");

            return Json(new { egitimAlmis , egitimGerekli , egitimDurumuIstekGonderildi, egitimbasladimi});
        }
    }

}





