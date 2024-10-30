using System.Linq.Expressions;

namespace LibraryManagementSystemV2.Services.Interfaces
{
    public interface IGenericService<TEntity, TShowDto, TCreateDto, TUpdateDto> : IReadService<TEntity, TShowDto>
        where TEntity : class
        where TShowDto : class
        where TCreateDto : class
        where TUpdateDto : class


    {
        Task<TShowDto> AddAsync(TCreateDto dto);
        Task DeleteAsync(long id);
        Task<TShowDto> UpdateAsync(long id, TUpdateDto dto);
    }
}
