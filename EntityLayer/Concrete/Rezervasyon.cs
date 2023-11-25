using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class Rezervasyon
    {
        [Key]
        public int Id { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string? Aciklama { get; set; }
        public bool BildirimAl { get; set; }
        public string? RezervasyonGuid { get; set; }
        public string? RezervasyonTuru { get; set; }
        public bool RezervasyonDurumu { get; set; }      
        public string? UserId { get; set; }
        public DateTime RezervasyonKayitTarihi { get; set; }
        public string? UserName { get; set; }
        public int CihazId { get; set; }       
        public virtual Cihazlar? cihaz { get; set; }
        public ICollection<Fiyatlandirma> Fiyatlandirma { get; set; }


    }
}
