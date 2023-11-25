namespace ProtaWebPortal.Models
{
	public class CihazListeModel
	{
		public int Id { get; set; }
		public string CihazAdi { get; set; }
		public string CihazModeli { get; set; }
		public string SeriNumarasi { get; set; }
		public string CihazinAlindigiYil { get; set; }
		public TimeSpan MinGunlukKullanim { get; set; }
		public TimeSpan MaxGunlukKullanim { get; set; }
		public string CihazGorseli { get; set; }
        public bool CihazDurum { get; set; }
        public DateTime CihazEklemeTarihi { get; set; }
    }
}
