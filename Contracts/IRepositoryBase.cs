using System;
using System.Linq;
using System.Linq.Expressions;

namespace Contracts
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(Expression<Func<T, bool>> express);
        IQueryable<T> FindByContrain(Expression<Func<T, bool>> express);
        void Insert(T entity);
        void Delete(T entity);
        void Edit(T entity);
    }
}
