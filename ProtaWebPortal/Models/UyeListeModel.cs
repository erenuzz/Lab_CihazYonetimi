namespace ProtaWebPortal.Models
{
    public class UyeListeModel
    {
      
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Email { get; set; }
        public DateTime KayitTarih { get; set; }
        public bool Durum { get; set; }
        public string KullaniciTipi { get; set; }
        public int TipId { get; set; }
        public int IndirimYuzdesi { get; set; }
        public string AdiSoyadi { get; set; }
    }
}
