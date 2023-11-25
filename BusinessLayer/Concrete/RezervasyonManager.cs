using BusinessLayer.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using EntityLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class RezervasyonManager : IRezervasyonService
    {


        IRezervasyonDal _rezervasyonDal;

        public RezervasyonManager(IRezervasyonDal rezervasyonDal)
        {
            _rezervasyonDal = rezervasyonDal;
        }

        public bool RezervasyonCakisiyorMu(DateTime baslangicTarihi, DateTime bitisTarihi)
        {
            using var c = new ProtaDbContext();            
            var cakisanRezervasyonlar = c.rezervasyons
                .Where(r => (r.BaslangicTarihi <= bitisTarihi && r.BitisTarihi >= baslangicTarihi))
                .ToList();            
            if (cakisanRezervasyonlar.Count > 0)
            {
                return true;
            }
                       
            return false;
        }


        public void TAdd(Rezervasyon t)
        {
            _rezervasyonDal.Insert(t);
        }

        public void TDelete(Rezervasyon t)
        {
            _rezervasyonDal.Delete(t);
        }

        public Rezervasyon TGetById(int id)
        {
            return _rezervasyonDal.GetById(id);
        }

        public IQueryable<Rezervasyon> TGetList(Expression<Func<Rezervasyon, bool>>? predicate=null)
        {
            return _rezervasyonDal.GetList(predicate);
        }

        public void TUpdate(Rezervasyon T)
        {
            _rezervasyonDal.Update(T);
        }
    }
}
