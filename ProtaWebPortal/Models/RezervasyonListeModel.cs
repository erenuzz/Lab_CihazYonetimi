namespace ProtaWebPortal.Models
{
    public class RezervasyonListeModel
    {
        public int Id { get; set; }
        public string? KullaniciAdi { get; set; }
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string CihazAdi { get; set; }
        public decimal Fiyat { get; set; }
        public string Guid { get; set; }
        public string RezervasyonTuru { get; set; }

        public bool RezervasyonDurumu { get; set; }
    }
}
