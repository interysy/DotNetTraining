using AutoMapper;
using Humanizer;
using LibraryManagementSystemV2.Repositories;
using LibraryManagementSystemV2.Services.Interfaces;
using System.Linq.Expressions;

namespace LibraryManagementSystemV2.Services.GenericServices
{
    public class GenericService<TEntity, TShowDto, TCreateDto, TUpdateDto> : ReadService<TEntity, TShowDto>, IGenericService<TEntity, TShowDto, TCreateDto, TUpdateDto>
        where TEntity : class
        where TShowDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GenericService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<TShowDto> AddAsync(TCreateDto dto)
        {
            await _unitOfWork.Repository<TEntity>().AddAsync(_mapper.Map<TEntity>(dto));
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TShowDto>(dto);
        }

        public async Task DeleteAsync(long id)
        {
            await _unitOfWork.Repository<TEntity>().DeleteByIdAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TShowDto> UpdateAsync(long id, TUpdateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _unitOfWork.Repository<TEntity>().UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TShowDto>(entity);
        }
    }
}
