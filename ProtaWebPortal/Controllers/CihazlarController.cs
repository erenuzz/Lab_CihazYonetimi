using BusinessLayer.Concrete;
using ClosedXML.Excel;
using DataAccess.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtaWebPortal.Models;
using SelectPdf;
using X.PagedList;

namespace ProtaWebPortal.Controllers
{
    [Authorize]
    public class CihazlarController : Controller
    {

        private readonly CihazManager _cihazManager;
        private readonly RezervasyonManager _rezervasyonManager;
        private readonly FiyatlandirmaManager _fiyatlandirmaManager;
        private readonly UserManager<AppUser> _userManager;
        public CihazlarController(ProtaDbContext c, CihazManager cihazManager, RezervasyonManager rezervasyonManager, FiyatlandirmaManager fiyatlandirmaManager,UserManager<AppUser> userManager)
        {
            _cihazManager = cihazManager;
            _rezervasyonManager = rezervasyonManager;
            _fiyatlandirmaManager = fiyatlandirmaManager;
            _userManager = userManager;
        }

        #region Index
        public IActionResult Index(int page = 1, string searchTerm = "", bool showAll = false)
        {
            var values = _cihazManager.TGetList().ToPagedList(page, 10);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowercaseSearchTerm = searchTerm.ToLower();
                values = values.Where(c => c.CihazAdi.ToLower().Contains(lowercaseSearchTerm)).ToPagedList(page, 10);
            }

            if (showAll)
            {
                // "Tümünü Göster" butonuna tıklanırsa, bütün verileri göster
                values = _cihazManager.TGetList().ToPagedList(page, 10);
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.ShowAll = showAll; // "Tümünü Göster" durumunu view'a gönder

            return View(values);
        }
        #endregion

        #region Cihaz Listele
        public IActionResult CihazListele()
        {
            var values = _cihazManager.TGetList(x=>x.CihazDurum == true);
            return Json(values);
        }
        #endregion

        #region CihazEkle
        [HttpGet]
        public IActionResult CihazEkle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CihazEkle(Cihazlar cihaz, IFormFile CihazGorseli)
        {
            string userName = HttpContext.Session.GetString("UserName");
            Cihazlar yenicihaz = new Cihazlar();
            if (CihazGorseli != null)
            {
                var extension = Path.GetExtension(CihazGorseli.FileName);
                var newimagename = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CihazGorselleri/", newimagename);
                var stream = new FileStream(location, FileMode.Create);
                CihazGorseli.CopyTo(stream);
                yenicihaz.CihazGorseli = "/CihazGorselleri/" + newimagename;
            }

            yenicihaz.CihazAdi = cihaz.CihazAdi;
            yenicihaz.CihazModeli = cihaz.CihazModeli;
            yenicihaz.SeriNumarasi = cihaz.SeriNumarasi;
            yenicihaz.CihazinAlindigiYil = cihaz.CihazinAlindigiYil;
            yenicihaz.MinGunlukKullanim = cihaz.MinGunlukKullanim;
            yenicihaz.MaxGunlukKullanim = cihaz.MaxGunlukKullanim;
            yenicihaz.CihazEklemeTarihi = DateTime.Now;
            yenicihaz.CihazDurum = true;
            yenicihaz.EgitimGerekliMi = cihaz.EgitimGerekliMi;
            yenicihaz.SaatlikFiyat = cihaz.SaatlikFiyat;
            _cihazManager.TAdd(yenicihaz);
            string logMessage = "Yeni cihaz eklendi: " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }

            return Redirect("/Cihazlar/Index/");

        }
        #endregion

        #region CihazSil
        public IActionResult CihazSil(int id)
        {
            string userName = HttpContext.Session.GetString("UserName");
            var values = _cihazManager.TGetById(id);
            _cihazManager.TDelete(values);

            string logMessage = "Cihaz silindi. " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }

            return Redirect("/Cihazlar/Index");
        }
        #endregion

        #region Guncelleme
        [HttpGet]
        public IActionResult CihazDuzenle(int id)
        {
            var values = _cihazManager.TGetById(id);
            return View(values);
        }
        [HttpPost]
        public IActionResult CihazDuzenle(Cihazlar cihaz, int id, IFormFile CihazGorseli)
        {
            string userName = HttpContext.Session.GetString("UserName");
            var values = _cihazManager.TGetById(id);
            if (CihazGorseli != null)
            {
                var extension = Path.GetExtension(CihazGorseli.FileName);
                var newimagename = Guid.NewGuid() + extension;
                var location = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CihazGorselleri/", newimagename);
                var stream = new FileStream(location, FileMode.Create);
                CihazGorseli.CopyTo(stream);
                values.CihazGorseli = "/CihazGorselleri/" + newimagename;
            }
            else
            {
                values.CihazGorseli = values.CihazGorseli;
            }
            values.CihazAdi = cihaz.CihazAdi;
            values.CihazModeli = cihaz.CihazModeli;
            values.SeriNumarasi = cihaz.SeriNumarasi;
            values.CihazinAlindigiYil = cihaz.CihazinAlindigiYil;
            values.MinGunlukKullanim = cihaz.MinGunlukKullanim;
            values.EgitimGerekliMi = cihaz.EgitimGerekliMi;
            values.MaxGunlukKullanim = cihaz.MaxGunlukKullanim;
            values.CihazDurum = values.CihazDurum;
            values.CihazEklemeTarihi = values.CihazEklemeTarihi;
            values.SaatlikFiyat = cihaz.SaatlikFiyat;
            _cihazManager.TUpdate(values);

            string logMessage = "Cihaz güncellendi. " + "İşlemi yapan kullanıcı adı: " + " " + userName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }

            return Redirect("/Cihazlar/Index");
        }
        #endregion

        #region CihazRezervasyonBul
        public IActionResult CihazRezervasyonBul(int id)
        {
            var values = _rezervasyonManager.TGetList(x => x.CihazId == id).OrderByDescending(x => x.RezervasyonKayitTarihi).Include(x => x.Fiyatlandirma)
                .Select(x => new CihazRezervasyonGecmisi
                {
                    BaslangicTarihi = x.BaslangicTarihi,
                    BitisTarihi = x.BitisTarihi,
                    Fiyati = x.Fiyatlandirma.FirstOrDefault().ToplamFiyat,
                    KullaniciAdi = x.UserName
                });
            return Json(values);
        }
        #endregion

        #region Excel
        public IActionResult Excel()
        {

            using (var excel = new XLWorkbook())
            {

                var values = _cihazManager.TGetList();

                var worksheet = excel.Worksheets.Add("Cihazlar");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Cihaz Adı";
                worksheet.Cell(currentRow, 2).Value = "Cihaz Modeli";
                worksheet.Cell(currentRow, 3).Value = "Cihaz Seri Numarası";
                worksheet.Cell(currentRow, 4).Value = "Cihazın Alındığı Yıl";
                worksheet.Cell(currentRow, 5).Value = "Min. Günlük Kullanım";
                worksheet.Cell(currentRow, 6).Value = "Max. Günlük Kullanım";
                worksheet.Cell(currentRow, 7).Value = "Saatlik Fiyat";

                foreach (var item in values)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.CihazAdi;
                    worksheet.Cell(currentRow, 2).Value = item.CihazModeli;
                    worksheet.Cell(currentRow, 3).Value = item.SeriNumarasi;
                    worksheet.Cell(currentRow, 4).Value = item.CihazinAlindigiYil;
                    worksheet.Cell(currentRow, 5).Value = item.MinGunlukKullanim;
                    worksheet.Cell(currentRow, 6).Value = item.MaxGunlukKullanim;
                    worksheet.Cell(currentRow, 7).Value = item.SaatlikFiyat + "₺";
                }

                using (var stream = new MemoryStream())
                {
                    excel.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "cihazlar.xlsx");
                }
            }


        }
        #endregion

        #region Pdf
        public IActionResult Pdf()
        {
            var values = _cihazManager.TGetList();

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Cihazlar");
            var currentRow = 1;
            worksheet.Cell(currentRow, 1).Value = "Cihaz Adı";
            worksheet.Cell(currentRow, 2).Value = "Cihaz Modeli";
            worksheet.Cell(currentRow, 3).Value = "Cihaz Seri Numarası";
            worksheet.Cell(currentRow, 4).Value = "Cihazın Alındığı Yıl";
            worksheet.Cell(currentRow, 5).Value = "Min. Günlük Kullanım";
            worksheet.Cell(currentRow, 6).Value = "Max. Günlük Kullanım";
            worksheet.Cell(currentRow, 7).Value = "Saatlik Fiyat";

            foreach (var item in values)
            {
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = item.CihazAdi;
                worksheet.Cell(currentRow, 2).Value = item.CihazModeli;
                worksheet.Cell(currentRow, 3).Value = item.SeriNumarasi;
                worksheet.Cell(currentRow, 4).Value = item.CihazinAlindigiYil;
                worksheet.Cell(currentRow, 5).Value = item.MinGunlukKullanim;
                worksheet.Cell(currentRow, 6).Value = item.MaxGunlukKullanim;
                worksheet.Cell(currentRow, 7).Value = item.SaatlikFiyat + "₺";
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
                "<th style='border: 1px solid #ddd; padding: 8px;'>Cihaz Adı</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Cihaz Modeli</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Cihaz Seri Numarası</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Cihazın Alındığı Yıl</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Min. Günlük Kullanım</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Max. Günlük Kullanım</th>" +
                "<th style='border: 1px solid #ddd; padding: 8px;'>Saatlik Fiyat</th>" +
                "</tr></thead><tbody>";

                htmlContent += string.Join("", values.Select(item =>
                    "<tr>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.CihazAdi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.CihazModeli}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.SeriNumarasi}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.CihazinAlindigiYil}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.MinGunlukKullanim}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.MaxGunlukKullanim}</td>" +
                    $"<td style='border: 1px solid #ddd; padding: 8px;'>{item.SaatlikFiyat}₺</td>" +
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
                        "cihazlar.pdf");
                }
            }
        }

        #endregion

        #region CihazınDurumunuDüzenle
        [HttpPost]
        public async Task<IActionResult> CihazDurumDuzenle(int cihazId, bool durum)
        {
            var cihaz = _cihazManager.TGetById(cihazId);
            cihaz.CihazDurum = durum;
            _cihazManager.TUpdate(cihaz);
            var rezervasyonBul = _rezervasyonManager.TGetList(x => x.CihazId == cihazId);
            var kullaniciMail = await _userManager.FindByIdAsync(rezervasyonBul.FirstOrDefault().UserId);
            if (rezervasyonBul != null)
            {
                Mail mail = new Mail();
                foreach (var item in rezervasyonBul.ToList())
                {
                    item.RezervasyonDurumu = durum;
                    _rezervasyonManager.TUpdate(item);
                    if (durum == false)
                    {
                        mail.MailGonder("Rezervasyon İptali", kullaniciMail.UserName, kullaniciMail.Isim + " " + kullaniciMail.Soyisim, $"Rezervasyon aldığınız {cihaz.CihazAdi} isimli cihaz pasif duruma getirildi ve {item.BaslangicTarihi} {item.BitisTarihi} tarihleri arasında bulunan rezervasyonunuz iptal edildi.");
                    }

                }
            }

            string logMessage = "Cihaz durumu değiştirildi. " + "İşlemi yapan kullanıcı adı: " + " " + kullaniciMail.UserName + " " + " Tarih: " + DateTime.Now;
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "log.txt");

            using (StreamWriter sw = new StreamWriter(logFilePath, true))
            {
                sw.WriteLine(logMessage);
            }

            return Json(cihaz);

        } 
        #endregion

    }
}
