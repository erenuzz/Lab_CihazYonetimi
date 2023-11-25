using EntityLayer.Concrete;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class ProtaDbContext : IdentityDbContext<AppUser,AppRole,int>
    {


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=217.116.199.169;database=ProtaWebPortalDb; Persist Security Info=True; User ID=Prota;Password=Piksel6650.;");
          
        }

       
        public DbSet<Cihazlar> cihazlars { get; set; }
        public DbSet<Rezervasyon> rezervasyons { get; set; }
        public DbSet<KullaniciTurleri> kullaniciTurleris { get; set; }
        public DbSet<Fiyatlandirma> fiyatlandirmas { get; set; }       
        public DbSet<Egitimler> egitimlers { get; set; }
        public DbSet<Mentor> mentors { get; set; }
        public DbSet<MentorEgitimleri> mentorEgitimleris { get; set; }
        public DbSet<KullaniciveMentorEgitimleri> kullaniciveMentorEgitimleris { get; set; }
    }
}

//ProtaWebPortalDb
//Prota
//Piksel6650.