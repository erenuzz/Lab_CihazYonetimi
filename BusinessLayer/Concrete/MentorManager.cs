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
	public class MentorManager : IMentorService
	{
		IMentorDal _mentor;
        public MentorManager(IMentorDal mentor)
        {
				_mentor = mentor;
        }
        public void TAdd(Mentor t)
		{
			_mentor.Insert(t);
		}

		public void TDelete(Mentor t)
		{
			_mentor.Delete(t);
		}

		public Mentor TGetById(int id)
		{
			return _mentor.GetById(id);
		}

		public IQueryable<Mentor> TGetList(Expression<Func<Mentor, bool>>? predicate = null)
		{
			return _mentor.GetList(predicate);
		}

		public void TUpdate(Mentor T)
		{
			_mentor.Update(T);
		}
	}
}
