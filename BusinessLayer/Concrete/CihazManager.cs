using BusinessLayer.Abstract;
using DataAccess.Abstract;
using DataAccess.EntityFramework;
using EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BusinessLayer.Concrete
{

    public class CihazManager : ICihazService
    {
        ICihazDal _cihazDal;

        public CihazManager(ICihazDal cihazDal)
        {
            _cihazDal = cihazDal;
        }

        public void TAdd(Cihazlar t)
        {
           _cihazDal.Insert(t);
        }

        public void TDelete(Cihazlar t)
        {
           _cihazDal.Delete(t);
        }

        public Cihazlar TGetById(int id)
        {
            return _cihazDal.GetById(id);
        }

        public IQueryable<Cihazlar> TGetList(Expression<Func<Cihazlar, bool>>? filter=null)
        {
            return _cihazDal.GetList(filter);
        }

        public void TUpdate(Cihazlar T)
        {
            _cihazDal.Update(T);
        }
    }
}
