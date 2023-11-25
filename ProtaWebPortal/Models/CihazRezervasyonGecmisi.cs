namespace ProtaWebPortal.Models
{
    public class CihazRezervasyonGecmisi
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal Fiyati { get; set; }
        public string KullaniciAdi { get; set; }
    }
}
