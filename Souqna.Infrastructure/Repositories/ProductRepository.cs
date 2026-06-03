using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Souqna.Application.DTOs;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Application.Interfaces.Services;
using Souqna.Domin.Entities;
using Souqna.Domin.Sharing;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepositories<Product>, IProductRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IImageManagementService imagemanagmentService;
        public ProductRepository(ApplicationDbContext context, IMapper mapper, IImageManagementService imagemanagmentService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imagemanagmentService = imagemanagmentService;
        }
        public async Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams)
        {
            var products = context.Products
                                  .Include(m => m.Category)
                                  .Include(m => m.Photos)
                                  .AsNoTracking()
                                  .Where(c => !c.IsDeleted);
            // Search functionality: split the search string into words and check if all words are contained in either the name or description
            if (!string.IsNullOrEmpty(productParams.Search))
            {
                var searchWord = productParams.Search.Split(' ');
                products=products.Where(m=>searchWord.All(word=>m.Name.ToLower().Contains(word.ToLower()) || m.Description.ToLower().Contains(word.ToLower())));
            }
            // Filter by category if CategoryId is provided
            if (productParams.CategoryId.HasValue)
            {
                products = products.Where(m => m.CategoryId == productParams.CategoryId);
            }
            // Sorting functionality based on the Sort parameter
            products = productParams.Sort switch
            {
                "priceAsc" => products.OrderBy(m => m.NewPrice),
                "priceDesc" => products.OrderByDescending(m => m.NewPrice),
                _ => products.OrderBy(m => m.Name),
            };
            // Pagination: skip and take based on the PageNumber and PageSize parameters
            products = products.Skip((productParams.PageNumber - 1) * productParams.PageSize)
                               .Take(productParams.PageSize);

            var result = await products.ToListAsync(); 
            return mapper.Map<IEnumerable<ProductDto>>(result);
        }

        public async Task<bool> AddAsync(AddProductDto productDto)
        {
            if (productDto is null)
            {
                throw new ArgumentNullException(nameof(productDto));
            }
            var product = mapper.Map<Product>(productDto);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            var imagePaths = await imagemanagmentService.UploadImageAsync(productDto.Photos, productDto.Name);
            var photos = imagePaths.Select(path => new Photo
            {
                ImageName = path,
                ProductId = product.Id
            }).ToList();

            if (photos.Any())
            {
                await context.Photos.AddRangeAsync(photos);
                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> UpdateAsync(UpdateProductDto productDto)
        {
            if (productDto is null)
            {
                return false;
            }
            var findProduct = await context.Products.Include(m => m.Category).Include(m => m.Photos)
                .FirstOrDefaultAsync(m => m.Id == productDto.Id);

            if (findProduct is null)
            {
                return false;
            }
            mapper.Map(productDto, findProduct);
            var findPhotos=await context.Photos.Where(m => m.ProductId == findProduct.Id).ToListAsync();
            foreach (var photo in findPhotos)
            {
                imagemanagmentService.DeleteImageAsync(photo.ImageName);

            }
            context.Photos.RemoveRange(findPhotos);
            var imagePaths = await imagemanagmentService.UploadImageAsync(productDto.Photos, productDto.Name);
            var photos = imagePaths.Select(path => new Photo
            {
                ImageName = path,
                ProductId = findProduct.Id
            }).ToList();
            await context.Photos.AddRangeAsync(photos);
            await context.SaveChangesAsync();
            return true;
        }

    }
}
