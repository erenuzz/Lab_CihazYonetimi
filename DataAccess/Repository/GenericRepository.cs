using DataAccess.Abstract;
using DataAccess.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace DataAccess.Repository
{
    public class GenericRepository<T> : IGenericDal<T> where T : class
    {
        private readonly ProtaDbContext _context= new ProtaDbContext();
        public GenericRepository()
        {
                _context = new ProtaDbContext();
        }

        public void Delete(T t)
        {
            _context.Remove(t);
           _context.SaveChanges();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public IQueryable<T> GetList(Expression<Func<T,bool>> pridicate)
        {

            var result= _context.Set<T>().AsNoTracking().AsQueryable();
            if (pridicate != null)
                result = result.Where(pridicate);
            
            return result;

        }

        public void Insert(T t)
        {
            _context.Add(t);
            _context.SaveChanges();
        }

      

        public void Update(T t)
        {
            
            _context.Update(t);
            _context.SaveChanges();
        }
    }
}
