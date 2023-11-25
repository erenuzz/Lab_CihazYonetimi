using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Concrete
{
    public class KullaniciveMentorEgitimleri
    {
        [Key]
        public int Id { get; set; }
        public int MentorEgitimleriId { get; set; }
        public virtual MentorEgitimleri MentorEgitimleri { get; set; }
        public int AppUserId { get; set; }
        public virtual AppUser AppUser { get; set; }    
    }
}
