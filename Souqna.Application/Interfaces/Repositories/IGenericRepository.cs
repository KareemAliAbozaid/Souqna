using Souqna.Domin.Entities;

namespace Souqna.Application.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(params System.Linq.Expressions.Expression<Func<T, object>>[] includes);
        Task<T?> GetByIdAsync(int id);
        Task<T?> GetByIdAsync(int id, params System.Linq.Expressions.Expression<Func<T, object>>[] includes);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<int> CountAsync();
    }
}
