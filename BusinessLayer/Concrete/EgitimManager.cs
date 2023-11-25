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
    public class EgitimManager : IEgitimService
    {
        IEgitimDal _egitimDal;

        public EgitimManager(IEgitimDal egitimDal)
        {
            _egitimDal = egitimDal;
        }

        public void TAdd(Egitimler t)
        {
            _egitimDal.Insert(t);
        }

        public void TDelete(Egitimler t)
        {
           _egitimDal.Delete(t);
        }

        public Egitimler TGetById(int id)
        {
            return _egitimDal.GetById(id);
        }

        public IQueryable<Egitimler> TGetList(Expression<Func<Egitimler, bool>>? filter = null)
        {
            return _egitimDal.GetList(filter);
        }

        public void TUpdate(Egitimler T)
        {
           _egitimDal.Update(T);
        }
    }
}
