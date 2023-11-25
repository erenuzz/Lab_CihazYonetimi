namespace ProtaWebPortal.Models
{
    public class RezervasyonEkleModel
    {
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public string Aciklama { get; set; }
        public bool RezervasyonDurumu { get; set; }
        public int HaftalikRezervasyon { get; set; }
        public string CihazAdi { get; set; }
        public string? RezervasyonTuru { get; set; }
        public string UserId { get; set; }
        public DateTime RezervasyonKayitTarihi { get; set; }
    }
}
