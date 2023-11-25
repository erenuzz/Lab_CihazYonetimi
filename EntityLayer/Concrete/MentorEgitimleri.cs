using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class MentorEgitimleri
    {
        public int Id { get; set; }
        public string EgitimAdi { get; set; }
        public string Suresi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public int? Kontenjan { get; set; }
        public string? EgitimKapakFotografi { get; set; }
        public int? MentorId { get; set; }
        public virtual Mentor Mentor { get; set; }

    }
}
