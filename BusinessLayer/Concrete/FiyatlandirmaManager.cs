using BusinessLayer.Abstract;
using DataAccess.Abstract;
using DataAccess.Repository;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class FiyatlandirmaManager : IFiyatlandirmaService
    {
        IFiyatlandirmaDal _fiyatlandirmaDal;

        public FiyatlandirmaManager(IFiyatlandirmaDal fiyatlandirmaDal)
        {
            _fiyatlandirmaDal = fiyatlandirmaDal;
        }

        public void TAdd(Fiyatlandirma t)
        {
           _fiyatlandirmaDal.Insert(t);
        }

        public void TDelete(Fiyatlandirma t)
        {
            _fiyatlandirmaDal.Delete(t);
        }

        public Fiyatlandirma TGetById(int id)
        {
           return _fiyatlandirmaDal.GetById(id);
        }

        public IQueryable<Fiyatlandirma> TGetList(Expression<Func<Fiyatlandirma, bool>>? predicate=null)
        {
            return _fiyatlandirmaDal.GetList(predicate);
        }

        public void TUpdate(Fiyatlandirma T)
        {
            _fiyatlandirmaDal.Update(T);
        }
    }
}
