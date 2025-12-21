using Microsoft.EntityFrameworkCore;
using Souqna.Domin.Interfaces;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class GenericRepositories<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        public GenericRepositories(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()=>await _context.Set<T>().AsNoTracking().ToListAsync();


        public async Task<IReadOnlyList<T>> GetAllAsync(params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
           var query = _context.Set<T>().AsQueryable();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
           var entity = await _context.Set<T>().FindAsync(id);
           return entity;
        }

        public Task<T?> GetByIdAsync(int id, params System.Linq.Expressions.Expression<Func<T, object>>[] includes)
        {
         IQueryable<T> query = _context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
        public async Task<T> AddAsync(T entity)
        { 
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
             return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await  _context.Set<T>().FindAsync(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
         _context.Set<T>().Update(entity);
         await _context.SaveChangesAsync();
        }
    }
}
