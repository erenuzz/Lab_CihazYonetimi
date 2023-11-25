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
    public class MentorEgitimManager : IMentorEgitimService
    {
        IMentorEgitimDal _mentoregitim;
        public MentorEgitimManager(IMentorEgitimDal mentorEgitimDal)
        {
            _mentoregitim = mentorEgitimDal;
        }
        public void TAdd(MentorEgitimleri t)
        {
            _mentoregitim.Insert(t);
        }

        public void TDelete(MentorEgitimleri t)
        {
            _mentoregitim.Delete(t);
        }

        public MentorEgitimleri TGetById(int id)
        {
            return _mentoregitim.GetById(id);
        }

        public IQueryable<MentorEgitimleri> TGetList(Expression<Func<MentorEgitimleri, bool>>? predicate=null)
        {
            return _mentoregitim.GetList(predicate);
        }

        public void TUpdate(MentorEgitimleri T)
        {
            _mentoregitim.Update(T);
        }
    }
}
