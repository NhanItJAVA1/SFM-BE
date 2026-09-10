using SFM_BE.Contexts;
using SFM_BE.Repositories.Generic;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class
    {
        var type = typeof(TEntity);

        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new GenericRepository<TEntity>(_context);
            _repositories.Add(type, repository);
        }

        return (IGenericRepository<TEntity>)repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
