using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
	public class Mentor
	{
		[Key]
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public string UzmanlikAlani { get; set; }
        public DateTime KayitTarihi { get; set; }
        //public int AppUserId { get; set; }
        //public virtual AppUser AppUser { get; set; }
    }
}
