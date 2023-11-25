namespace ProtaWebPortal.Models
{
    public class KullaniciVeMentorEgitimleriModel
    {
        public int Id { get; set; }
        public string EgitimAdi { get; set; }
        public string Suresi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string MentorAdiSoyadi { get; set; }
    }
}
