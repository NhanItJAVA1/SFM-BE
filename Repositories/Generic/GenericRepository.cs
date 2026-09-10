using Microsoft.EntityFrameworkCore;
using SFM_BE.Contexts;
using System.Linq.Expressions;

namespace SFM_BE.Repositories.Generic;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(AppDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public IQueryable<T> All()
    {
        return DbSet;
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }

    public IQueryable<T> WhereInclude(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = DbSet.Where(predicate);

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

        return query;
    }

    public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.FirstOrDefaultAsync(predicate);
    }

    public async Task<T> CreateAsync(T item)
    {
        await DbSet.AddAsync(item);
        return item;
    }

    public async Task CreateRangeAsync(IEnumerable<T> items)
    {
        await DbSet.AddRangeAsync(items);
    }

    public Task<T> UpdateAsync(T item)
    {
        DbSet.Update(item);
        return Task.FromResult(item);
    }

    public Task UpdateRangeAsync(IEnumerable<T> items)
    {
        DbSet.UpdateRange(items);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T item)
    {
        DbSet.Remove(item);
        return Task.CompletedTask;
    }

    public void DeleteRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
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
