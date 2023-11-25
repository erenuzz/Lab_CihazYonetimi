namespace ProtaWebPortal.Models
{
    public class EgitimIstekleriListe
    {
        public int Id { get; set; }
        public string CihazAdi { get; set; }
        public string KullaniciAdi { get; set; }
        public bool EgitimDurumu { get; set; }
        public string EgitimNeDurumda { get; set; }
    }
}
