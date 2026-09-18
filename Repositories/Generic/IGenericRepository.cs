using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace SFM_BE.Repositories.Generic;

public interface IGenericRepository<T>
    where T : class
{
    IQueryable<T> All();
    IQueryable<T> Where(Expression<Func<T, bool>> predicate);
    IQueryable<T> WhereInclude(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

    Task<T?> FindByIdAsync(long id);

    Task<T> CreateAsync(T item);

    Task CreateRangeAsync(IEnumerable<T> items);

    Task<T> UpdateAsync(T item);
    Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Action<UpdateSettersBuilder<T>> set);
    Task UpdateRangeAsync(IEnumerable<T> items);

    void Delete(T item);
    void DeleteById(long  id);
    Task<int> DeleteAsync(Expression<Func<T, bool>> predicate);
    void DeleteRange(IEnumerable<T> entities);

    void SetOriginalValue<TProperty>(
        T entity,
        Expression<Func<T, TProperty>> property,
        TProperty value);
}
