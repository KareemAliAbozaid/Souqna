# Onion Architecture - Practical Implementation Examples

## Example 1: How to Add a New Feature (Category Management)

### Flow Diagram
```
API Request ? Controller ? Application Service ? Repository ? Database
                ?            ?                      ?
            (HTTP)      (Business Logic)      (Data Access)
                ?            ?                      ?
           API Returns   Maps to DTOs         Maps to Entities
```

### Detailed Example: GetAllCategories

#### 1. API Layer (Souqna.API/Controllers/CategoriesController.cs)
```csharp
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Souqna.API.Helper;
using Souqna.Application.DTOs;                    // ? Application layer DTOs
using Souqna.Application.Interfaces.Repositories; // ? Application layer interfaces

namespace Souqna.API.Controllers
{
    public class CategoriesController : BaseController
    {
        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                // Call Application interface (implemented by Infrastructure)
                var categories = await unitOfWork.Categories.GetAllAsync();
                
                // Map Domain entities to Application DTOs
                var categoryDtos = mapper.Map<IEnumerable<CategoryDto>>(categories);
                
                // Return Application DTO in response
                return Ok(new ResponseApiResponse<IEnumerable<CategoryDto>>(200, categoryDtos));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseApi(500, ex.Message));
            }
        }
    }
}
```

#### 2. Application Layer
**Interface (Souqna.Application/Interfaces/Repositories/ICategoryRepository.cs):**
```csharp
using Souqna.Domin.Entities;

namespace Souqna.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Inherits: GetAllAsync(), GetByIdAsync(), AddAsync(), UpdateAsync(), DeleteAsync()
    }
}
```

**DTO (Souqna.Application/DTOs/CategoryDto.cs):**
```csharp
namespace Souqna.Application.DTOs
{
    public record CategoryDto(string Name, string? Description);
    public record UpdateCategoryDto(int Id, string Name, string? Description);
}
```

**Mapper (Souqna.Application/Mappings/CategoryMappingProfile.cs):**
```csharp
using AutoMapper;
using Souqna.Application.DTOs;
using Souqna.Domin.Entities;

namespace Souqna.Application.Mappings
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // Domain Entity ? Application DTO
            CreateMap<Category, CategoryDto>()
                .ConstructUsing(src => new CategoryDto(src.Name, src.Description))
                .ReverseMap()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());

            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());
        }
    }
}
```

#### 3. Domain Layer (Souqna.Domin/Entities/Category.cs)
```csharp
namespace Souqna.Domin.Entities
{
    // Pure domain entity - NO external dependencies
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
```

#### 4. Infrastructure Layer
**Implementation (Souqna.Infrastructure/Repositories/CategoryRepository.cs):**
```csharp
using Souqna.Domin.Entities;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    // Implements Application interface
    public class CategoryRepository : GenericRepositories<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        // Inherits all methods from GenericRepositories<Category>
        // which implements IGenericRepository<Category>
    }
}
```

**Unit of Work (Souqna.Infrastructure/Repositories/UnitOfWork.cs):**
```csharp
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

        public UnitOfWork(
            ApplicationDbContext context, 
            IMapper mapper, 
            IImageManagementService imageManagementService)
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
```

**DbContext (Souqna.Infrastructure/Data/ApplicationDbContext.cs):**
```csharp
using Microsoft.EntityFrameworkCore;
using Souqna.Domin.Entities;

namespace Souqna.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Entity configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
```

---

## Example 2: Product Management (More Complex)

### Adding a Product with Images

#### Flow
```
POST /api/products (MultipartFormData)
    ?
ProductsController.AddProduct(AddProductDto)
    ?
IProductRepository.AddAsync(AddProductDto)
    ?
[Infrastructure] ProductRepository.AddAsync(AddProductDto)
    ?? Map AddProductDto ? Product (Domain Entity)
    ?? Save Product to DB
    ?? Call IImageManagementService.UploadImageAsync()
    ?? Create Photo entities
    ?? Save Photos to DB
    ?
Return ResponseApi(200, "Added Successfully")
```

#### Implementation

**Application Layer - DTO (Souqna.Application/DTOs/ProductDto.cs):**
```csharp
using Microsoft.AspNetCore.Http;

namespace Souqna.Application.DTOs
{
    public record ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public string CategoryName { get; set; }
        public virtual ICollection<PhotoDto> Photos { get; set; }
    }

    public record PhotoDto
    {
        public string ImageName { get; set; }
        public int ProductId { get; set; }
    }

    public record AddProductDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public decimal NewPrice { get; set; }
        public decimal OldPrice { get; set; }
        public int CategoryId { get; set; }
        public IFormFileCollection Photos { get; set; }
    }

    public record UpdateProductDto : AddProductDto
    {
        public int Id { get; set; }
    }
}
```

**Application Layer - Interface (Souqna.Application/Interfaces/Repositories/IProductRepository.cs):**
```csharp
using Souqna.Application.DTOs;
using Souqna.Domin.Entities;
using Souqna.Domin.Sharing;

namespace Souqna.Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams);
        Task<bool> AddAsync(AddProductDto productDto);
        Task<bool> UpdateAsync(UpdateProductDto productDto);
    }
}
```

**Application Layer - Service Interface (Souqna.Application/Interfaces/Services/IImageManagementService.cs):**
```csharp
using Microsoft.AspNetCore.Http;

namespace Souqna.Application.Interfaces.Services
{
    public interface IImageManagementService
    {
        Task<List<string>> UploadImageAsync(IFormFileCollection files, string src);
        void DeleteImageAsync(string src);
    }
}
```

**Infrastructure - Service Implementation (Souqna.Infrastructure/Repositories/Service/ImageManagementService.cs):**
```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Souqna.Application.Interfaces.Services;

namespace Souqna.Infrastructure.Repositories.Service
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IFileProvider fileProvider;

        public ImageManagementService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

        public async Task<List<string>> UploadImageAsync(IFormFileCollection files, string src)
        {
            List<string> savedImagePaths = new List<string>();
            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", src);
            
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var savePath = Path.Combine(imageDirectory, fileName);
                    var imageUrl = $"/Images/{src}/{fileName}";

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedImagePaths.Add(imageUrl);
                }
            }

            return savedImagePaths;
        }

        public void DeleteImageAsync(string relativePath)
        {
            var fullPath = Path.Combine(
                Directory.GetCurrentDirectory(), 
                "wwwroot", 
                relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else
            {
                throw new FileNotFoundException($"The file '{relativePath}' does not exist.");
            }
        }
    }
}
```

**Infrastructure - Repository Implementation (Souqna.Infrastructure/Repositories/ProductRepository.cs):**
```csharp
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
        private readonly IImageManagementService imageManagementService;

        public ProductRepository(
            ApplicationDbContext context, 
            IMapper mapper, 
            IImageManagementService imageManagementService) 
            : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageManagementService = imageManagementService;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams)
        {
            var products = context.Products
                .Include(m => m.Category)
                .Include(m => m.Photos)
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

            if (productParams.CategoryId.HasValue)
            {
                products = products.Where(m => m.CategoryId == productParams.CategoryId);
            }

            products = productParams.Sort switch
            {
                "priceAsc" => products.OrderBy(m => m.NewPrice),
                "priceDesc" => products.OrderByDescending(m => m.NewPrice),
                _ => products.OrderBy(m => m.Name),
            };

            products = products.Skip((productParams.PageNumber - 1) * productParams.PageSize)
                .Take(productParams.PageSize);

            var result = await products.ToListAsync();
            return mapper.Map<IEnumerable<ProductDto>>(result);
        }

        public async Task<bool> AddAsync(AddProductDto productDto)
        {
            if (productDto is null)
                throw new ArgumentNullException(nameof(productDto));

            // Map Application DTO ? Domain Entity
            var product = mapper.Map<Product>(productDto);
            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            // Upload images using Application service interface
            var imagePaths = await imageManagementService.UploadImageAsync(productDto.Photos, productDto.Name);
            
            // Create Photo entities
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
                return false;

            var findProduct = await context.Products
                .Include(m => m.Category)
                .Include(m => m.Photos)
                .FirstOrDefaultAsync(m => m.Id == productDto.Id);

            if (findProduct is null)
                return false;

            mapper.Map(productDto, findProduct);
            
            var findPhotos = await context.Photos
                .Where(m => m.ProductId == findProduct.Id)
                .ToListAsync();
                
            foreach (var photo in findPhotos)
            {
                imageManagementService.DeleteImageAsync(photo.ImageName);
            }

            context.Photos.RemoveRange(findPhotos);
            
            var imagePaths = await imageManagementService.UploadImageAsync(productDto.Photos, productDto.Name);
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
```

**API Layer - Controller (Souqna.API/Controllers/ProductsController.cs):**
```csharp
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Souqna.API.Helper;
using Souqna.Application.DTOs;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Domin.Sharing;

namespace Souqna.API.Controllers
{
    public class ProductsController : BaseController
    {
        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper) 
            : base(unitOfWork, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
        {
            try
            {
                var products = await unitOfWork.Products.GetAllAsync(productParams);
                var count = await unitOfWork.Products.CountAsync();
                var pagination = new Pagination<ProductDto>(
                    products, 
                    productParams.PageNumber, 
                    productParams.PageSize, 
                    count);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] AddProductDto addProductDto)
        {
            try
            {
                await unitOfWork.Products.AddAsync(addProductDto);
                return Ok(new ResponseApi(200, "Added Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromForm] UpdateProductDto updateProductDto)
        {
            try
            {
                var isUpdated = await unitOfWork.Products.UpdateAsync(updateProductDto);
                if (!isUpdated)
                    return NotFound(new ResponseApi(404, "Product Not Found"));

                return Ok(new ResponseApi(200, "Updated Successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
```

---

## Example 3: Dependency Injection Setup

**Souqna.Application/ApplicationServiceRegistration.cs:**
```csharp
using Microsoft.Extensions.DependencyInjection;

namespace Souqna.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper profiles from this assembly
            services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);

            return services;
        }
    }
}
```

**Souqna.Infrastructure/InfrastructureRegisteration.cs:**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Souqna.Application;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Application.Interfaces.Services;
using Souqna.Infrastructure.Repositories;
using Souqna.Infrastructure.Repositories.Service;

namespace Souqna.Infrastructure
{
    public static class InfrastructureRegisteration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // 1. Register Application services
            services.AddApplicationServices();

            // 2. Register Repository implementations
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepositories<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // 3. Register DbContext
            services.AddDbContext<Data.ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseSqlServer(connectionString);
            });

            // 4. Register External Services
            services.AddSingleton<IImageManagementService, ImageManagementService>();

            // 5. Register File Provider
            var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            if (!Directory.Exists(wwwrootPath))
            {
                Directory.CreateDirectory(wwwrootPath);
            }
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));

            return services;
        }
    }
}
```

**Souqna.API/Program.cs:**
```csharp
using Souqna.Infrastructure;

namespace Souqna.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add API services
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMemoryCache();

            // Add Infrastructure services (which includes Application services)
            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            // Configure HTTP pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/erroes/{0}");
            app.UseHttpsRedirection();
            app.UseMiddleware<Middleware.ExptionsMiddleware>();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
```

---

## Testing Example

### Unit Test for ProductRepository

```csharp
using Moq;
using Souqna.Application.DTOs;
using Souqna.Application.Interfaces.Services;
using Souqna.Domin.Entities;
using Souqna.Domin.Sharing;
using Souqna.Infrastructure.Data;
using Souqna.Infrastructure.Repositories;
using Xunit;

public class ProductRepositoryTests
{
    [Fact]
    public async Task AddAsync_ShouldAddProductAndPhotos()
    {
        // Arrange
        var mockContext = new Mock<ApplicationDbContext>();
        var mockMapper = new Mock<IMapper>();
        var mockImageService = new Mock<IImageManagementService>();

        var product = new Product { Id = 1, Name = "Test Product" };
        var addDto = new AddProductDto { Name = "Test Product", CategoryId = 1 };
        
        mockMapper.Setup(m => m.Map<Product>(addDto)).Returns(product);
        mockImageService.Setup(s => s.UploadImageAsync(It.IsAny<IFormFileCollection>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { "/Images/test/image.jpg" });

        var repository = new ProductRepository(mockContext.Object, mockMapper.Object, mockImageService.Object);

        // Act
        var result = await repository.AddAsync(addDto);

        // Assert
        Assert.True(result);
        mockImageService.Verify(s => s.UploadImageAsync(It.IsAny<IFormFileCollection>(), It.IsAny<string>()), Times.Once);
    }
}
```

---

## Summary

This Onion Architecture provides:
- **Clear separation of concerns** - each layer has a specific responsibility
- **High testability** - mock Application interfaces in Infrastructure layer
- **Reusability** - Domain layer can be used in different contexts
- **Maintainability** - easy to understand data flow and dependencies
