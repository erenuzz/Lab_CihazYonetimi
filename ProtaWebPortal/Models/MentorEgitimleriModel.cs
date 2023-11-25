﻿using EntityLayer.Concrete;

namespace ProtaWebPortal.Models
{
    public class MentorEgitimleriModel
    {
        public int Id { get; set; }
        public string EgitimAdi { get; set; }
        public string Suresi { get; set; }
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string? EgitimKapakFotografi { get; set; }
        public int? Kontenjan { get; set; }
        public string MentorAdi { get; set; }
        public int? MentorId { get; set; }
    }
}
