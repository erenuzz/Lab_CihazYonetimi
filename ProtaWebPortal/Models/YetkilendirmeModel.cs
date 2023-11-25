using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ProtaWebPortal.Models
{
    public class YetkilendirmeModel
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
        public bool Exists { get; set; }

        
    }
}
