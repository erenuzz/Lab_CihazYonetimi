namespace ProtaWebPortal.Models
{
	public class MentorListeModel
	{
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string EMail { get; set; }
        public string UzmanlikAlani { get; set; }
        public string KullaniciAdi { get; set; }
        public int KullaniciId { get; set; }
        public DateTime KayitTarihi { get; set; }
    }
}
