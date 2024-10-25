namespace LibraryManagementSystemV2.Services
{
    public interface IReadService<TEntity, TDto> where TEntity : class where TDto : class
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(long id);
    }
}
