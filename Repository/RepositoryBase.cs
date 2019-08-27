using System;
using Contracts;
using System.Linq;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T: class
    {
        private AppMeoContext _db;
        public RepositoryBase(AppMeoContext db)
        {
            _db = db;
        }

        public void Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
        }

        public void Edit(T entity)
        {
            _db.Set<T>().Update(entity);
        }

        public IQueryable<T> FindAll(System.Linq.Expressions.Expression<Func<T, bool>> express)
        {
            return _db.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByContrain(System.Linq.Expressions.Expression<Func<T, bool>> express)
        {
            return _db.Set<T>().Where(express).AsNoTracking();
        }

        public void Insert(T entity)
        {
            _db.Set<T>().Add(entity);
        }
    }
}
