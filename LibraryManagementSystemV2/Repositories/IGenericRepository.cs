using System.Linq.Expressions;

namespace LibraryManagementSystemV2.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(long id);
        Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includes);
        Task<List<T?>> GetAllAsync(bool tracked = true);

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> where = null, bool tracked = true, params Expression<Func<T, object>>[] includes);
        Task UpdateAsync(T entity);
        Task DeleteByIdAsync(long id);
        Task SaveAsync();
    }
}
