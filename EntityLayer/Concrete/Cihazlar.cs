using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
	public class Cihazlar
	{
		[Key]
		public int Id { get; set; }
        public string? CihazAdi { get; set; }       
        public string? CihazModeli { get; set; }
        public string? SeriNumarasi { get; set; }
        public string? CihazinAlindigiYil { get; set; }
        public TimeSpan MinGunlukKullanim { get; set; }
        public TimeSpan MaxGunlukKullanim { get; set; }
        public string? CihazGorseli { get; set; }
        public decimal SaatlikFiyat { get; set; }
        public bool CihazDurum { get; set; }
        public DateTime CihazEklemeTarihi { get; set; }
        public bool EgitimGerekliMi { get; set; }
    }
}
