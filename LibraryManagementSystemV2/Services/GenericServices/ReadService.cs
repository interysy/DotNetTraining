using AutoMapper;
using LibraryManagementSystemV2.Repositories.Interfaces;
using LibraryManagementSystemV2.Services.Interfaces;

namespace LibraryManagementSystemV2.Services.GenericServices
{
    public class ReadService<TEntity, TDto> : IReadService<TEntity, TDto>
        where TEntity : class
        where TDto : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public ReadService(IUnitOfWork unitOfWork, IMapper mapper) : base()
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IEnumerable<TDto>> GetAllAsync()
        {
            //try
            //{
            var result = await _unitOfWork.Repository<TEntity>().GetAllAsync();

            if (result.Any())
            {
                return _mapper.Map<IEnumerable<TDto>>(result);
            }
            else
            {
                throw new Exception($"No {typeof(TDto).Name}s were found");
            }

            //}
            //catch (EntityNotFoundException ex)
            //{
            //    var message = $"Error retrieving all {typeof(TDto).Name}s";

            //    throw new EntityNotFoundException(message, ex);
            //}
        }

        public async Task<TDto> GetByIdAsync(long id)
        {
            //try
            //{
            var result = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);

            if (result is null)
            {
                throw new Exception($"Entity with ID {id} not found.");
            }

            return _mapper.Map<TDto>(result);
            //}

            //catch (EntityNotFoundException ex)
            //{
            //    var message = $"Error retrieving {typeof(TDto).Name} with Id: {id}";

            //    throw new EntityNotFoundException(message, ex);
            //}
        }
    }
}
