using BusinessLayer.Concrete;
using ClosedXML.Excel;
using DataAccess.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ProtaWebPortal.Models;
using SelectPdf;
using System.Reflection;
using X.PagedList;

namespace ProtaWebPortal.Controllers
{
   
    public class UyelerController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ProtaDbContext _C;
        private readonly IHttpContextAccessor _http;
        private readonly FiyatlandirmaManager _fiyatlirmaManager;
        private readonly RezervasyonManager _rezervasyonManager;
        private readonly CihazManager _cihazManager;
        private readonly EgitimManager _egitim;

        public UyelerController(UserManager<AppUser> userManager, ProtaDbContext c, RoleManager<AppRole> roleManager, IHttpContextAccessor http,FiyatlandirmaManager fiyatlandirmaManager, RezervasyonManager rezervasyonManager, CihazManager cihazManager, EgitimManager egitim)
        {
            _userManager = userManager;
            _C = c;
            _roleManager = roleManager;
            _http = http;
            _fiyatlirmaManager = fiyatlandirmaManager;
            _rezervasyonManager = rezervasyonManager;
            _cihazManager = cihazManager;
            _egitim = egitim;
        }

        public IActionResult Index(int page = 1, string uyefilter = "" ,  bool showAll = false)
        {
            if (!string.IsNullOrEmpty(uyefilter))
            {
                var values = _C.Users.Select(x => new UyeListeModel
                {
                    Id = x.Id,
                    Adi = x.Isim,  
                    Soyadi = x.Soyisim,
                    Email = x.UserName,
                    KayitTarih = x.KayitTarihi,
                    Durum = x.Durum,
                    KullaniciTipi = x.KullaniciTurleri.KullaniciTuru,
                    TipId = x.KullaniciTurleriId,
                    IndirimYuzdesi = x.IndirimYuzdesi,
                    AdiSoyadi = x.Isim + " " + x.Soyisim

                }).Where(x => x.Durum == true).OrderByDescending(x => x.KayitTarih).ToPagedList(page, 10);
                var lowerUyeFilter = uyefilter.ToLower();
                values = values.Where(x => x.AdiSoyadi.ToLower().Contains(lowerUyeFilter)).ToPagedList(page, 10);
                return View(values);
            }

            else if (showAll)
            {
                var values = _C.Users.Select(x => new UyeListeModel
                {
                    Id = x.Id,
                    Adi = x.Isim,
                    Soyadi = x.Soyisim,
                    Email = x.UserName,
                    KayitTarih = x.KayitTarihi,
                    Durum = x.Durum,
                    KullaniciTipi = x.KullaniciTurleri.KullaniciTuru,
                    TipId = x.KullaniciTurleriId,
                    IndirimYuzdesi = x.IndirimYuzdesi

                }).Where(x => x.Durum == true).OrderByDescending(x => x.KayitTarih).ToPagedList(page, 10);

                return View(values);
            }

            else
            {
                var values = _C.Users.Select(x => new UyeListeModel
                {
                    Id = x.Id,
                    Adi = x.Isim,
                    Soyadi = x.Soyisim,
                    Email = x.UserName,
                    KayitTarih = x.KayitTarihi,
                    Durum = x.Durum,
                    KullaniciTipi = x.KullaniciTurleri.KullaniciTuru,
                    TipId = x.KullaniciTurleriId,
                    IndirimYuzdesi = x.IndirimYuzdesi

                }).Where(x => x.Durum == true).OrderByDescending(x => x.KayitTarih).ToPagedList(page, 10);

                ViewBag.filter = uyefilter;
                ViewBag.ShowAll = showAll; // "Tümünü Göster" durumunu view'a gönder
                return View(values);
            }

        }

        public IActionResult UyeListele()
        {

            var values = _C.Users.Select(x => new UyeListeModel
            {
                Id = x.Id,
                Adi = x.Isim,
                Email = x.UserName,
                KayitTarih = x.KayitTarihi,
                Durum = x.Durum,
                KullaniciTipi = x.KullaniciTurleri.KullaniciTuru,
                TipId = x.KullaniciTurleriId,
                IndirimYuzdesi = x.IndirimYuzdesi,
                AdiSoyadi = x.Isim + " " + x.Soyisim

            });

            return Json(values);
        }
       
        public async Task<IActionResult> KullaniciPasiflestir(string id)
        {
            try
            {
                string username = HttpContext.Session.GetString("UserName");
                var result = await _userManager.FindByIdAsync(id);
                if (result != null)
                {
                    result.Durum = false;
                    await _userManager.UpdateAsync(result);
                    _C.SaveChanges();

                    string logMessage = "Üye pasifleştirme işlemi yapıldı: " + "İşlemi yapan kullanıcı adı: " + " " + username + " " + "Hangi üye pasifleştirildi:" + " / " + result.UserName + " Tarih: " + DateTime.Now;                   
                    string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

                    using (StreamWriter sw = new StreamWriter(logFilePath, true))
                    {
                        sw.WriteLine(logMessage);
                    }
                }
                return Redirect("/Uyeler/Index/");
            }
            catch (Exception)
            {
                TempData["hata"] = "Beklenmedik bir hata ile karşılaşıldı.";
                return View();
            }

        }

     
      
        [HttpGet]
        public async Task<IActionResult> Yetkilendirme(int id)
        {
            var user = _userManager.Users.FirstOrDefault
                 (x => x.Id == id);

            var roles = _roleManager.Roles.ToList();

            TempData["Userid"] = user.Id;

            var userRoles = await _userManager.GetRolesAsync(user);

            List<YetkilendirmeModel> yetkiModel = new List<YetkilendirmeModel>();

            foreach (var item in roles)
            {
                YetkilendirmeModel m = new YetkilendirmeModel();
                m.RoleID = item.Id;
                m.Name = item.Name;
                m.Exists = userRoles.Contains(item.Name);
                yetkiModel.Add(m);
            }

            return View(yetkiModel);
        }

        [HttpPost]
        public async Task<IActionResult> Yetkilendirme(List<YetkilendirmeModel> model)
        {
            string username = HttpContext.Session.GetString("UserName");
            int userid =(int)TempData["Userid"];
            var user = _userManager.Users.FirstOrDefault(x => x.Id == userid);
            foreach (var item in model)
            {
                if (item.Exists)
                {
                    await _userManager.AddToRoleAsync(user, item.Name);
                    string logMessage = "Kullanıcıya yetki verildi: " + "Yetkiyi veren kişi:" + " " + username + " " + "Yetki verilen kullanıcı adı:" + " " + user.UserName + "  " + "Verilen yetkinin adı:" + " " + item.Name + " " + " Tarih: " + DateTime.Now;                 

                    string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

                    using (StreamWriter sw = new StreamWriter(logFilePath, true))
                    {
                        sw.WriteLine(logMessage);
                    }

                    TempData["yetkimesaj"] = $"{user.UserName} isimli kullanıcıya {item.Name} yetkisi verildi.";
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, item.Name);

                }

            }
            return Redirect("/Uyeler/Index/");
        }
        
        public IActionResult KullaniciTipiListele()
        {
            var values = _C.kullaniciTurleris.ToList();
            return Json(values);
        }

        [HttpPost]
        public IActionResult KullaniciTipiDuzenleme(int id ,int TurId , int indirimYuzdesi)
        {
            string username = _http.HttpContext.Session.GetString("UserName");
            var values = _C.Users.Find(id);
            if (TurId != 0 && indirimYuzdesi != 0 && id != 0)
            {
                values.KullaniciTurleriId = TurId;
                values.IndirimYuzdesi = indirimYuzdesi;
                _C.Update(values);
                _C.SaveChanges();

                if (values.IndirimYuzdesi != 0)
                {
                    var fiyatBul = _fiyatlirmaManager.TGetList(x => x.appUserId == id).Include(x=>x.rezervasyon).ToList();
                    
                    foreach (var item in fiyatBul)
                    {
                        var cihaz = _cihazManager.TGetById(item.cihazId);
                        
                        var rezervasyonBaslangicTarihi = item.rezervasyon.BaslangicTarihi.Hour;
                        var rezervasyonBitisTarihi = item.rezervasyon.BitisTarihi.Hour;
                        var toplamSaat = rezervasyonBitisTarihi - rezervasyonBaslangicTarihi;

                        var cihazSaatlikFiyat = cihaz.SaatlikFiyat;
                        var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * toplamSaat;

                        var yeniIndirim = indirimYuzdesi;
                        var indirimliFiyat = cihazToplamRezervasyonFiyati * yeniIndirim / 100;
                        var geneltutar = cihazToplamRezervasyonFiyati - indirimliFiyat;

                        item.ToplamFiyat = geneltutar;
                        item.IndirimliFiyat = indirimliFiyat;
                       
                        _fiyatlirmaManager.TUpdate(item);
                    }

                }

            }
            else
            {
                values.KullaniciTurleriId = values.KullaniciTurleriId;
                values.IndirimYuzdesi = indirimYuzdesi;
                _C.Update(values);
                _C.SaveChanges();

                if (values.IndirimYuzdesi == 0)
                {
                    var fiyatBul = _fiyatlirmaManager.TGetList(x => x.appUserId == id).Include(x => x.rezervasyon).ToList();

                    foreach (var item in fiyatBul)
                    {
                        var cihaz = _cihazManager.TGetById(item.cihazId);

                        var rezervasyonBaslangicTarihi = item.rezervasyon.BaslangicTarihi.Hour;
                        var rezervasyonBitisTarihi = item.rezervasyon.BitisTarihi.Hour;
                        var toplamSaat = rezervasyonBitisTarihi - rezervasyonBaslangicTarihi;

                        var cihazSaatlikFiyat = cihaz.SaatlikFiyat;
                        var cihazToplamRezervasyonFiyati = cihazSaatlikFiyat * toplamSaat;

                        item.ToplamFiyat = cihazToplamRezervasyonFiyati;
                        item.IndirimliFiyat = indirimYuzdesi;

                        _fiyatlirmaManager.TUpdate(item);
                    }

                }
            }

            string logMessage = "Kullanıcı bilgilerinde değişiklik yapıldı: " + "İşlemi yapan kullanıcı adı: " + " " + username + " " + " / " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");
            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }
            return Redirect("/Uyeler/Index/");
        }

        public IActionResult UyeRezervasyonBul(string id)
        {
            var values = _rezervasyonManager.TGetList(x=>x.UserId == id).OrderByDescending(x => x.RezervasyonKayitTarihi).Include(x => x.Fiyatlandirma)
                .Select(x => new UyeRezervasyonGecmisi
                {
                    BaslangicTarihi = x.BaslangicTarihi,
                    BitisTarihi = x.BitisTarihi,
                    Fiyati = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                    KullaniciAdi = x.UserName
                });
           
            return Json(values);
        }


        public IActionResult Excel()
        {

            using (var excel = new XLWorkbook())
            {
                               

                var values = _C.Users.Select(x => new UyeListeModel
                {
                    Id = x.Id,
                    Adi = x.Isim,
                    Soyadi = x.Soyisim,
                    Email = x.UserName,
                    KayitTarih = x.KayitTarihi,
                    Durum = x.Durum,
                    KullaniciTipi = x.KullaniciTurleri.KullaniciTuru,
                    TipId = x.KullaniciTurleriId,
                    IndirimYuzdesi = x.IndirimYuzdesi

                });



                var worksheet = excel.Worksheets.Add("Uyeler");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Adı";
                worksheet.Cell(currentRow, 2).Value = "Soyadı";
                worksheet.Cell(currentRow, 3).Value = "Email";
                worksheet.Cell(currentRow, 4).Value = "Kayıt Tarihi";
                worksheet.Cell(currentRow, 5).Value = "Kullanıcı Tipi";
                worksheet.Cell(currentRow, 6).Value = "İndirim Yüzdesi";

                foreach (var item in values)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.Adi;
                    worksheet.Cell(currentRow, 2).Value = item.Soyadi;
                    worksheet.Cell(currentRow, 3).Value = item.Email;
                    worksheet.Cell(currentRow, 4).Value = item.KayitTarih;
                    worksheet.Cell(currentRow, 5).Value = item.KullaniciTipi;
                    worksheet.Cell(currentRow, 6).Value = item.IndirimYuzdesi + "%";
                }

                using (var stream = new MemoryStream())
                {
                    excel.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "uyeler.xlsx");
                }
            }


        }

        public IActionResult Pdf()
        {
            var values = _C.Users.Select(x => new UyeListeModel
            {
                Id = x.Id,
                Adi = x.Isim,
                Soyadi = x.Soyisim,
                Email = x.UserName,
                KayitTarih = x.KayitTarihi,
                Durum = x.Durum,
                KullaniciTipi = x.KullaniciTurleri.KullaniciTuru,
                TipId = x.KullaniciTurleriId,
                IndirimYuzdesi = x.IndirimYuzdesi

            });

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Uyeler");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Adı";
            worksheet.Cell(currentRow, 2).Value = "Soyadı";
            worksheet.Cell(currentRow, 3).Value = "Email";
            worksheet.Cell(currentRow, 4).Value = "Kayıt Tarihi";
            worksheet.Cell(currentRow, 5).Value = "Kullanıcı Tipi";
            worksheet.Cell(currentRow, 6).Value = "İndirim Yüzdesi";

            foreach (var item in values)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = item.Adi;
                worksheet.Cell(currentRow, 2).Value = item.Soyadi;
                worksheet.Cell(currentRow, 3).Value = item.Email;
                worksheet.Cell(currentRow, 4).Value = item.KayitTarih;
                worksheet.Cell(currentRow, 5).Value = item.KullaniciTipi;
                worksheet.Cell(currentRow, 6).Value = item.IndirimYuzdesi + "%";
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
                "<th style='border: 1px solid #ddd; padding: 8px;'>Adı</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Soyadı</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Email</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Kayıt Tarihi</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Kullanıcı Tipi</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>İndirim Yüzdesi</th>" +
                "</tr></thead><tbody>";

                htmlContent += string.Join("", values.Select(item =>
                    "<tr>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Adi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Soyadi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.Email}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.KayitTarih}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.KullaniciTipi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.IndirimYuzdesi}</td>" +
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
                        "uyeler.pdf");
                }
            }
        }

        public IActionResult UyeEgitimleri(int id)
        {
            var values = _egitim.TGetList(x => x.appUserId == id).Include(x => x.cihaz)
                .Select(x => new UyeEgitimleri
                {
                    CihazAdi = x.cihaz.CihazAdi,
                    EgitimDurumu = x.EgitimNeDurumda
                });
            return Json(values);
        }

    }
}

