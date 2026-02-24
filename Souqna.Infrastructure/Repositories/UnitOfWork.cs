using AutoMapper;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Application.Interfaces.Services;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageManagementService _imageManagementService;
        public ICategoryRepository Categories { get; }

        public IProductRepository Products { get; }

        public IPhotoRepository Photos { get; }

        public UnitOfWork(ApplicationDbContext context, IMapper mapper, IImageManagementService imageManagementService)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context, mapper, imageManagementService);
            Photos = new PhotoRepository(_context);
            _mapper = mapper;
            _imageManagementService = imageManagementService;
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }
    }
}