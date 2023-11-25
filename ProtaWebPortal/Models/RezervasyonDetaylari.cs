namespace ProtaWebPortal.Models
{
    public class RezervasyonDetaylari
    {
        public int RezervasyonId { get; set; }
        public string CihazAdi { get; set; }
        public decimal IndirimliFiyat { get; set; }
        public DateTime RezervasyonBaslangicTarihi { get; set; }
        public DateTime RezervasyonBitisTarihi { get; set; }
        public decimal ToplamFiyat { get; set; }
        public int IndirimTutari { get; set; }
        public string KullaniciAdi { get; set; }
        public string CihazGorseli { get; set; }
        public decimal CihazSaatlikFiyat { get; set; }
    }
}
