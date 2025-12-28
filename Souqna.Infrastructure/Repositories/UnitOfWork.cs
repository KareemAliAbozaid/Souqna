using Souqna.Domin.Interfaces;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepository Categories { get; }

        public IProductRepository Products { get; }

        public IPhotoRepository Photos { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            Photos = new PhotoRepository(_context);
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }
    }
}