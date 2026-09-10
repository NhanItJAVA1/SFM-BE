using SFM_BE.Repositories.Generic;

namespace SFM_BE.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;

    Task<int> SaveChangesAsync();
}
