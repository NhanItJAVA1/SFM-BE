using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SFM_BE.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SFM_BE.Repositories.Generic;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> All()
    {
        return _dbSet;
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public IQueryable<T> WhereInclude(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet.Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return query;
    }

    public async Task<T?> FindByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> CreateAsync(T item)
    {
        await _dbSet.AddAsync(item);
        return item;
    }

    public async Task CreateRangeAsync(IEnumerable<T> items)
    {
        await _dbSet.AddRangeAsync(items);
    }

    public Task<T> UpdateAsync(T item)
    {
        _dbSet.Update(item);
        return Task.FromResult(item);
    }

    public async Task<int> UpdateAsync(Expression<Func<T, bool>> predicate, Action<UpdateSettersBuilder<T>> set)
    {
        return await _dbSet.Where(predicate).ExecuteUpdateAsync(set);
    }

    public Task UpdateRangeAsync(IEnumerable<T> items)
    {
        _dbSet.UpdateRange(items);
        return Task.CompletedTask;
    }

    public void Delete(T item)
    {
        _dbSet.Remove(item);
    }

    public void DeleteById(long id)
    {
        var entity = _dbSet.Find(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ExecuteDeleteAsync();
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public void SetOriginalValue<TProperty>(
        T entity,
        Expression<Func<T, TProperty>> property,
        TProperty value)
    {
        Context.Entry(entity)
            .Property(property)
            .OriginalValue = value;
    }
}
