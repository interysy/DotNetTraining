using System.Linq.Expressions;

namespace LibraryManagementSystemV2.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task<T?> GetByIdAsync(long id);
        Task<T?> GetByIdAsync(long id, params Expression<Func<T, object>>[] includes);
        Task<List<T>> GetAllAsync(bool tracked = true);
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? where = null, bool tracked = true, params Expression<Func<T, object>>[] includes);
        Task UpdateAsync(T entity);
        Task DeleteByIdAsync(long id);
        void DeleteRange(IEnumerable<T> toRemove);
        Task SaveAsync();
        void SetEntryStateToModified(T entry);
        bool Exists(Expression<Func<T, bool>> expression);
        Task ExplicitlyLoad<TProperty>(T entity, Expression<Func<T, IEnumerable<TProperty>>> navigationPropertyPath) where TProperty : class;
    }
}
