using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Fiyatlandirma
    {
        [Key]
        public int Id { get; set; }    
        public decimal ToplamFiyat { get; set; }
        public decimal IndirimliFiyat { get; set; }
        public int appUserId { get; set; }
        public virtual AppUser appUser { get; set; }

        public int cihazId { get; set; }
        public virtual Cihazlar cihaz { get; set; }

        public int rezervasyonId { get; set; }
        public virtual Rezervasyon rezervasyon { get; set; }
    }
}
