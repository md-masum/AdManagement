using System.Linq.Expressions;
using AdCore.Dto;
using AdCore.Entity;
using AdCore.Exceptions;
using AdCore.Extensions;
using AdRepository.Interface;
using AdService.Interface;
using AutoMapper;

namespace AdService.Base
{
    public class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto> where TEntity : BaseEntity where TDto:BaseDto
    {
        public readonly ICosmosDbRepository<TEntity> BaseRepository;
        private readonly IMapper _mapper;

        public BaseService(ICosmosDbRepository<TEntity> baseRepository, IMapper mapper)
        {
            BaseRepository = baseRepository;
            _mapper = mapper;
        }

        public virtual async Task<TDto> AddAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await BaseRepository.AddAsync(entity);
            return _mapper.Map<TDto>(entity);
        }
        
        public virtual async Task<TDto> UpdateAsync(string id, TDto dto)
        {
            if (dto == null) throw new CustomException("No data provided");
            var existing = await BaseRepository.GetAsync(id);
            if (existing == null) throw new NotFoundException("Data not found");
            existing.UpdateObject(dto);
            await BaseRepository.UpdateAsync(id, existing);
            return _mapper.Map<TDto>(existing);
        }
        
        public virtual async Task<bool> RemoveAsync(string id)
        {
            return await BaseRepository.DeleteAsync(id);
        }
        
        public virtual async Task<TDto> GetByIdAsync(string id)
        {
            var data = await BaseRepository.GetAsync(id);
            if (data is null)
            {
                throw new NotFoundException();
            }
            return _mapper.Map<TDto>(data);
        }

        public virtual async Task<IList<TDto>> GetAllAsync()
        {
            var data = await BaseRepository.GetAllAsync();
            return _mapper.Map<IList<TDto>>(data);
        }

        public virtual async Task<IList<TDto>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var data = await BaseRepository.GetAllAsync();
            var filteredData = data.AsQueryable().Where(predicate).ToList();
            return _mapper.Map<IList<TDto>>(filteredData);
        }

        public virtual async Task<TDto> GetAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var data = await BaseRepository.GetAllAsync();
            var filteredData = data.AsQueryable().Where(predicate).FirstOrDefault();
            return _mapper.Map<TDto>(filteredData);
        }
    }
}
