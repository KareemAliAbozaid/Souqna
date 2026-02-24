# Onion Architecture Migration - Final Setup Guide

## Current Status

Your project structure has been reorganized into 4 layers:

```
Souqna/
??? Souqna.API/              (Presentation Layer)
??? Souqna.Application/      (Application Layer) ? CREATED
??? Souqna.Domin/            (Domain Layer)
??? Souqna.Infrastructure/   (Infrastructure Layer)
??? Souqna.sln
```

## What's Been Done

### ? Created Application Layer
- **`Souqna.Application/Souqna.Application.csproj`** - New project file
- **Interfaces (Repositories & Services):**
  - `Interfaces/Repositories/IGenericRepository.cs`
  - `Interfaces/Repositories/ICategoryRepository.cs`
  - `Interfaces/Repositories/IProductRepository.cs`
  - `Interfaces/Repositories/IPhotoRepository.cs`
  - `Interfaces/Repositories/IUnitOfWork.cs`
  - `Interfaces/Services/IImageManagementService.cs`
- **DTOs:**
  - `DTOs/CategoryDto.cs`
  - `DTOs/ProductDto.cs`
- **AutoMapper Profiles:**
  - `Mappings/CategoryMappingProfile.cs`
  - `Mappings/ProductMappingProfile.cs`
- **Dependency Injection:**
  - `ApplicationServiceRegistration.cs`

### ? Updated Infrastructure Layer
- Updated `InfrastructureRegisteration.cs` to register Application services
- Updated all repositories to use Application interfaces
- Updated `UnitOfWork.cs` to use Application interfaces
- Updated `ImageManagementService.cs` to implement Application interface

### ? Cleaned Domain Layer
- ? Removed `Interfaces/` folder (moved to Application)
- ? Removed `DTOs/` folder (moved to Application)
- ? Removed `Services/` folder (moved to Application)
- ? Kept only entities: `Category.cs`, `Product.cs`, `Photo.cs`, `BaseEntity.cs`
- ? Kept shared models: `ProductParams.cs`
- ? Updated `Souqna.Domin.csproj` to remove unnecessary dependencies

### ? Updated API Layer
- Updated `Program.cs` to call `AddInfrastructureServices()`
- Updated `BaseController.cs` to use Application interfaces
- Updated `ProductsController.cs` to use Application DTOs and interfaces
- Updated `CategoriesController.cs` to use Application DTOs and interfaces
- Updated `BugsController.cs` to use Application interfaces
- ? Removed old mapping files (now in Application layer)
- ? Updated `Souqna.API.csproj` to reference both Application and Infrastructure

---

## Final Step: Add Application Project to Solution

The Application project has been created but needs to be added to the solution file. Run this command:

```bash
cd D:\Desktop\Souqna\Souqna
dotnet sln Souqna.sln add Souqna.Application\Souqna.Application.csproj
```

Or simply run the batch file I created:
```
setup-solution.bat
```

---

## Verification Checklist

After running the above command, verify the following:

### 1. Solution Structure
```bash
cd D:\Desktop\Souqna\Souqna
dotnet sln Souqna.sln list
```

Should output:
```
Project(s)
----------
Souqna.API
Souqna.Application  ? Should appear here
Souqna.Domin
Souqna.Infrastructure
```

### 2. Build Project
```bash
dotnet clean
dotnet restore
dotnet build
```

Should complete with **0 errors**.

### 3. Run Project
```bash
dotnet run --project Souqna.API
```

Application should start without errors.

---

## Project Dependencies (Onion Architecture)

```
Souqna.API
  ??> Souqna.Infrastructure (implements Application interfaces)
  ??> Souqna.Application (contains DTOs and interface contracts)

Souqna.Infrastructure
  ??> Souqna.Application (implements these interfaces)
  ??> Souqna.Domin (uses domain entities)

Souqna.Application
  ??> Souqna.Domin (only dependency is domain entities)

Souqna.Domin
  ??> (NO DEPENDENCIES) ? Pure domain logic
```

---

## File Organization Summary

### Souqna.API (Presentation Layer)
```
Controllers/
  ??? BaseController.cs           (uses IUnitOfWork from Application)
  ??? ProductsController.cs       (uses Application DTOs & interfaces)
  ??? CategoriesController.cs     (uses Application DTOs & interfaces)
  ??? BugsController.cs           (uses Application interfaces)
  ??? ErrorsController.cs

Helper/
  ??? ResponseApi.cs
  ??? Pagination.cs
  ??? ApiExceptions.cs

Middleware/
  ??? ExptionsMiddleware.cs

wwwroot/
Program.cs                         (registers Infrastructure services)
```

### Souqna.Application (Application Layer)
```
ApplicationServiceRegistration.cs  (registers AutoMapper)

DTOs/
  ??? CategoryDto.cs
  ??? ProductDto.cs

Interfaces/
  ??? Repositories/
  ?   ??? IGenericRepository.cs
  ?   ??? ICategoryRepository.cs
  ?   ??? IProductRepository.cs
  ?   ??? IPhotoRepository.cs
  ?   ??? IUnitOfWork.cs
  ??? Services/
      ??? IImageManagementService.cs

Mappings/
  ??? CategoryMappingProfile.cs
  ??? ProductMappingProfile.cs
```

### Souqna.Domin (Domain Layer)
```
Entities/
  ??? BaseEntity.cs
  ??? Category.cs
  ??? Product.cs
  ??? Photo.cs

Sharing/
  ??? ProductParams.cs
```

### Souqna.Infrastructure (Infrastructure Layer)
```
Data/
  ??? ApplicationDbContext.cs
  ??? Migrations/
  ??? Config/

Repositories/
  ??? GenericRepositories.cs
  ??? CategoryRepository.cs
  ??? ProductRepository.cs
  ??? PhotoRepository.cs
  ??? UnitOfWork.cs
  ??? Service/
      ??? ImageManagementService.cs

InfrastructureRegisteration.cs
```

---

## Data Flow Example: Get All Products

```
1. HTTP Request
   ?
2. ProductsController.GetAllProducts()
   - Dependency: IUnitOfWork (from Application interface)
   - IMapper (from AutoMapper registered in Application)
   ?
3. IUnitOfWork.Products.GetAllAsync(productParams)
   - This calls ProductRepository (Infrastructure implementation)
   ?
4. ProductRepository.GetAllAsync()
   - Queries Domain entities from DbContext
   - Maps Domain.Product ? Application.ProductDto
   ?
5. Return List<ProductDto>
   ?
6. API returns JSON response
```

---

## API Endpoints

All endpoints use **Application DTOs** for request/response:

### Products
- `GET /api/products` - Get all products (paginated, with filtering)
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create product (multipart form with images)
- `PUT /api/products` - Update product (multipart form with images)
- `DELETE /api/products/{id}` - Delete product (soft delete)

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/{id}` - Get category by ID
- `POST /api/categories` - Create category
- `PUT /api/categories/{id}` - Update category
- `DELETE /api/categories/{id}` - Delete category (soft delete)

---

## Testing the Architecture

### Unit Test Example (Testing a Repository)

```csharp
using Moq;
using Souqna.Application.DTOs;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Infrastructure.Repositories;
using Xunit;

public class ProductRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsProductDtos()
    {
        // Arrange - Mock Application interfaces
        var mockContext = new Mock<ApplicationDbContext>();
        var mockMapper = new Mock<IMapper>();
        var mockImageService = new Mock<IImageManagementService>();

        var repository = new ProductRepository(
            mockContext.Object, 
            mockMapper.Object, 
            mockImageService.Object
        );

        // Act
        var result = await repository.GetAllAsync(new ProductParams());

        // Assert
        Assert.NotNull(result);
        Assert.IsAssignableFrom<IEnumerable<ProductDto>>(result);
    }
}
```

---

## Troubleshooting

### Issue: "Application" namespace not found
**Solution:** Run `dotnet sln add` command to add project to solution, then reload solution in Visual Studio.

### Issue: Missing wwwroot directory
**Solution:** Infrastructure creates it automatically via `InfrastructureRegisteration.cs`, but you can manually create:
```
Souqna.API/
  ??? wwwroot/
      ??? Images/
```

### Issue: Build fails with project reference errors
**Solution:**
```bash
dotnet clean
dotnet restore
dotnet build
```

### Issue: AutoMapper profiles not found
**Solution:** Verify `ApplicationServiceRegistration.cs` calls:
```csharp
services.AddAutoMapper(typeof(ApplicationServiceRegistration).Assembly);
```

---

## Next Steps

1. **Run setup command** (add Application to solution)
2. **Build solution** (should have 0 errors)
3. **Run application** (should start without errors)
4. **Test endpoints** using Swagger/Postman
5. **Add more features** following the same Onion Architecture pattern

---

## Benefits Achieved

? **Clear Separation of Concerns** - Each layer has one responsibility
? **Testability** - Mock Application interfaces to test Infrastructure
? **Maintainability** - Easy to understand and modify
? **Scalability** - Easy to add new features following same pattern
? **Reusability** - Domain layer can be used in different contexts
? **Flexibility** - Easy to swap Infrastructure implementations

---

## Quick Reference

### Adding a New Entity

1. Create Entity in `Souqna.Domin/Entities/`
   ```csharp
   public class YourEntity : BaseEntity
   {
       // Properties
   }
   ```

2. Create DTOs in `Souqna.Application/DTOs/`
   ```csharp
   public record YourEntityDto(/* properties */);
   ```

3. Create Repository Interface in `Souqna.Application/Interfaces/Repositories/`
   ```csharp
   public interface IYourEntityRepository : IGenericRepository<YourEntity>
   {
   }
   ```

4. Create Mapping Profile in `Souqna.Application/Mappings/`
   ```csharp
   public class YourEntityMappingProfile : Profile
   {
       public YourEntityMappingProfile()
       {
           CreateMap<YourEntity, YourEntityDto>().ReverseMap();
       }
   }
   ```

5. Create Repository Implementation in `Souqna.Infrastructure/Repositories/`
   ```csharp
   public class YourEntityRepository : GenericRepositories<YourEntity>, IYourEntityRepository
   {
       public YourEntityRepository(ApplicationDbContext context) : base(context)
       {
       }
   }
   ```

6. Add DbSet in `Souqna.Infrastructure/Data/ApplicationDbContext.cs`
   ```csharp
   public DbSet<YourEntity> YourEntities { get; set; }
   ```

7. Register in `Souqna.Infrastructure/InfrastructureRegisteration.cs`
   ```csharp
   services.AddScoped<IYourEntityRepository, YourEntityRepository>();
   ```

8. Create Controller in `Souqna.API/Controllers/`
   ```csharp
   public class YourEntitiesController : BaseController
   {
       // Implementation using IUnitOfWork
   }
   ```

---

That's it! Your 4-layer Onion Architecture is ready. Just run the setup command and start building! ??
