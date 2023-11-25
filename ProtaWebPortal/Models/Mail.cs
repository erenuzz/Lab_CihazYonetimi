using System.Net.Mail;
using System.Net;
using BusinessLayer.Concrete;

namespace ProtaWebPortal.Models
{
    public class Mail
    {
       
        public void MailGonder(string konu="", string aliciMail="" , string aliciIsim="", string mesajIcerigi="")
        {
            MailMessage msg = new MailMessage(); //Mesaj gövdesini tanımlıyoruz...
            msg.Subject = konu;
            msg.From = new MailAddress("beoerp00@gmail.com", "Piksel Bilişim");
            msg.To.Add(new MailAddress(aliciMail, aliciIsim));                        
            msg.IsBodyHtml = true;
            msg.Body = mesajIcerigi;                       
            msg.Priority = MailPriority.High;

            //SMTP/Gönderici bilgilerinin yer aldığı erişim/doğrulama bilgileri
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587); //Bu alanda gönderim yapacak hizmetin smtp adresini ve size verilen portu girmelisiniz.
            NetworkCredential AccountInfo = new NetworkCredential("beoerp00@gmail.com", "zicihvcwnbiiugbw");
            smtp.UseDefaultCredentials = false; //Standart doğrulama kullanılsın mı? -> Yalnızca gönderici özellikle istiyor ise TRUE işaretlenir.
            smtp.Credentials = AccountInfo;
            smtp.EnableSsl = true; //SSL kullanılarak mı gönderilsin...
            smtp.Send(msg);
            
        }
                
    }
}
