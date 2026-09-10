using System.Linq.Expressions;

namespace SFM_BE.Repositories.Generic;

public interface IGenericRepository<T>
    where T : class
{
    IQueryable<T> All();

    IQueryable<T> Where(Expression<Func<T, bool>> predicate);

    IQueryable<T> WhereInclude(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties);

    Task<T?> FindAsync(Expression<Func<T, bool>> predicate);

    Task<T> CreateAsync(T item);

    Task CreateRangeAsync(IEnumerable<T> items);

    Task<T> UpdateAsync(T item);

    Task UpdateRangeAsync(IEnumerable<T> items);

    Task DeleteAsync(T item);

    void DeleteRange(IEnumerable<T> entities);

    void SetOriginalValue<TProperty>(
        T entity,
        Expression<Func<T, TProperty>> property,
        TProperty value);
}
