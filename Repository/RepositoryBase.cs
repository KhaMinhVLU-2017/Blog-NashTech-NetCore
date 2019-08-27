using System;
using Contracts;
using System.Linq;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Collections.Generic;

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

        public void DeleteRange(IEnumerable<T> ListEntity)
        {
            _db.Set<T>().RemoveRange(ListEntity);
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

        public T FindByID(int id)
        {
           return _db.Set<T>().Find(id);
        }

        public void Insert(T entity)
        {
            _db.Set<T>().Add(entity);
        }

        public IQueryable<TType> SelectCover<TType>(Expression<Func<T, TType>> express)
        {
            return _db.Set<T>().Select(express);
        }
    }
}
