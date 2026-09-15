using Microsoft.EntityFrameworkCore.Storage;
using SFM_BE.Repositories.Generic;
using System.Threading.Tasks;

namespace SFM_BE.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> GetRepository<TEntity>()
        where TEntity : class;
    Task<IDbContextTransaction> BeginTransactionAsync();

    Task<int> SaveChangesAsync();
}

