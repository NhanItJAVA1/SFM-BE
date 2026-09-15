using Microsoft.EntityFrameworkCore;
using SFM_BE.Entities.Interface;
using SFM_BE.Enums;

namespace SFM_BE.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WithDeleted<T>(this IQueryable<T> query)
            where T : class => query.IgnoreQueryFilters();

        public static IQueryable<T> DeletedOnly<T>(this IQueryable<T> query)
            where T : class, ISoftDeletable => query.IgnoreQueryFilters().Where(x => x.DeletedAt != null);

        public static IQueryable<T> ExcludeDeleted<T>(this IQueryable<T> query)
            where T : ISoftDeletable => query.Where(x => x.DeletedAt == null);

        public static IQueryable<T> DeleteFilter<T>(this IQueryable<T> query, DeleteType filter)
            where T : ISoftDeletable => filter switch
            {
                DeleteType.NotDeleted => query.Where(x => x.DeletedAt == null),
                DeleteType.Deleted => query.Where(x => x.DeletedAt != null),
                _ => query
            };
    }
}
