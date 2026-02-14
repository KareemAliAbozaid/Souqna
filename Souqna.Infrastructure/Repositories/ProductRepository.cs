using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Souqna.Domin.DTOs;
using Souqna.Domin.Entities;
using Souqna.Domin.Interfaces;
using Souqna.Domin.Services;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepositories<Product>, IProductRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IImagemanagmentService imagemanagmentService;
        public ProductRepository(ApplicationDbContext context, IMapper mapper, IImagemanagmentService imagemanagmentService) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imagemanagmentService = imagemanagmentService;
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

        //public async Task DeleteAsync(Product product)
        //{
        //    var photo=await context.Photos.Where(m => m.ProductId == product.Id).ToListAsync();
        //    foreach (var item in photo)
        //    {
        //        imagemanagmentService.DeleteImageAsync(item.ImageName);
        //    }
        //    context.Photos.RemoveRange(photo);
        //    await context.SaveChangesAsync();
        
            
        //}
    }
}
