using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class AppUser:IdentityUser<int>
    {
     
        public string Isim { get; set; }
        public string Soyisim { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool Durum { get; set; }
        public int IndirimYuzdesi { get; set; }
        public int KullaniciTurleriId { get; set; }

        public virtual KullaniciTurleri KullaniciTurleri { get; set; }
       
    }
}
