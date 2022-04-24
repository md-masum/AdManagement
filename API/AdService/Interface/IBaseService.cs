using System.Linq.Expressions;
using AdCore.Entity;

namespace AdService.Interface
{
    public interface IBaseService<TEntity, TDto> where TEntity : BaseEntity
    {
        //HTTP GET LIST
        Task<IList<TDto>> GetAllAsync();
        Task<IList<TDto>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);

        //HTTP GET SINGLE
        Task<TDto> GetByIdAsync(string id);
        Task<TDto> GetAsync(Expression<Func<TEntity, bool>> predicate);

        //HTTP POST
        Task<TDto> AddAsync(TDto entity);

        //HTTP PUT
        Task<TDto> UpdateAsync(string id, TDto entity);

        //HTTP DELETE
        Task<bool> RemoveAsync(string id);
    }
}
