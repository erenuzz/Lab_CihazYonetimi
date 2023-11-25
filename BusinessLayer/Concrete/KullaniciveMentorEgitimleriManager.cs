using BusinessLayer.Abstract;
using DataAccess.Abstract;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class KullaniciveMentorEgitimleriManager : IKullaniciveMentoEgitimleriService
    {
        IKullaniciveMentorEgitimleriDal _kullanicivementoregitimleridal;

        public KullaniciveMentorEgitimleriManager(IKullaniciveMentorEgitimleriDal kullanicivementoregitimleridal)
        {
            _kullanicivementoregitimleridal = kullanicivementoregitimleridal;
        }

        public void TAdd(KullaniciveMentorEgitimleri t)
        {
            _kullanicivementoregitimleridal.Insert(t);
        }

        public void TDelete(KullaniciveMentorEgitimleri t)
        {
           _kullanicivementoregitimleridal.Delete(t);
        }

        public KullaniciveMentorEgitimleri TGetById(int id)
        {
            return _kullanicivementoregitimleridal.GetById(id);
        }

        public IQueryable<KullaniciveMentorEgitimleri> TGetList(Expression<Func<KullaniciveMentorEgitimleri, bool>>? predicate=null)
        {
            return _kullanicivementoregitimleridal.GetList(predicate);
        }

        public void TUpdate(KullaniciveMentorEgitimleri T)
        {
           _kullanicivementoregitimleridal.Update(T);
        }
    }
}
