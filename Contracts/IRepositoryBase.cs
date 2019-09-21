using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IRepositoryBase<T>
    {
        IEnumerable<T> FindAll();
        T FindByID(int id);
        IQueryable<T> FindByContrain(Expression<Func<T, bool>> express);
        IQueryable<TType> SelectCover<TType>(Expression<Func<T, TType>> express);
        void Insert(T entity);
        void Delete(T entity);
        void DeleteById(int Id);
        void Edit(T entity);
        void DeleteRange(IEnumerable<T> entity);
        void UpdateEntity(T entity);
    }
}
