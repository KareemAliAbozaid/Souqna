using AutoMapper;
using Souqna.Domin.Interfaces;
using Souqna.Domin.Services;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImagemanagmentService _imagemanagmentService;
        public ICategoryRepository Categories { get; }

        public IProductRepository Products { get; }

        public IPhotoRepository Photos { get; }

        public UnitOfWork(ApplicationDbContext context, IMapper mapper, IImagemanagmentService imagemanagmentService)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context,mapper,imagemanagmentService);
            Photos = new PhotoRepository(_context);
            _mapper = mapper;
            _imagemanagmentService = imagemanagmentService;
        }

        public async Task<bool> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }
    }
}