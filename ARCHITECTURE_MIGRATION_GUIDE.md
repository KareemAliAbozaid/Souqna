# Onion Architecture Migration Guide for Souqna Project

## Overview
Your project is being migrated from a 3-layer architecture to a proper Onion Architecture with 4 layers.

## Layer Structure

```
Souqna.API (Presentation Layer)
    ? depends on
Souqna.Application (Application Layer)
    ? depends on
Souqna.Domin (Domain Layer)

Souqna.Infrastructure (Infrastructure Layer)
    ? implements interfaces from
Souqna.Application
    ? uses entities from
Souqna.Domin
```

## Layer Responsibilities

### 1. **Souqna.Domin** (Domain Layer - No Dependencies)
- **Pure business entities** (no framework dependencies)
- **Value objects** and domain logic
- **Base classes**: `BaseEntity`
- **Entities**:
  - `Category.cs`
  - `Product.cs`
  - `Photo.cs`
- **Shared Models**: `ProductParams.cs`

**What should NOT be here:**
- ? DTOs (moved to Application)
- ? Repository interfaces (moved to Application)
- ? Service interfaces (moved to Application)
- ? AutoMapper, EF, or any external dependencies

---

### 2. **Souqna.Application** (Application Layer - Only Domain Dependency)
- **DTOs** for data transfer
- **Repository interfaces** (contract definitions)
- **Service interfaces** (application services contract)
- **AutoMapper profiles** (for mapping DTOs)
- **Application service registration**

**Structure:**
```
Souqna.Application/
??? ApplicationServiceRegistration.cs
??? DTOs/
?   ??? CategoryDto.cs
?   ??? ProductDto.cs
??? Interfaces/
?   ??? Repositories/
?   ?   ??? IGenericRepository.cs
?   ?   ??? ICategoryRepository.cs
?   ?   ??? IProductRepository.cs
?   ?   ??? IPhotoRepository.cs
?   ?   ??? IUnitOfWork.cs
?   ??? Services/
?       ??? IImageManagementService.cs
??? Mappings/
    ??? CategoryMappingProfile.cs
    ??? ProductMappingProfile.cs
```

---

### 3. **Souqna.Infrastructure** (Infrastructure Layer - Implements Application Contracts)
- **Repository implementations** (uses Application interfaces)
- **Database context** and migrations
- **External service implementations** (`ImageManagementService`)
- **Dependency injection registration**

**Structure:**
```
Souqna.Infrastructure/
??? InfrastructureRegisteration.cs
??? Data/
?   ??? ApplicationDbContext.cs
?   ??? Config/
?   ??? Migrations/
??? Repositories/
?   ??? GenericRepositories.cs
?   ??? CategoryRepository.cs
?   ??? ProductRepository.cs
?   ??? PhotoRepository.cs
?   ??? UnitOfWork.cs
?   ??? Service/
?       ??? ImageManagementService.cs
```

---

### 4. **Souqna.API** (Presentation Layer - Only Application Dependency)
- **REST Controllers**
- **API request/response models** (using Application DTOs)
- **Middleware** and exception handling
- **Swagger/OpenAPI configuration**

**Structure:**
```
Souqna.API/
??? Program.cs (DI Registration)
??? Controllers/
?   ??? BaseController.cs
?   ??? ProductsController.cs
?   ??? CategoriesController.cs
?   ??? ErrorsController.cs
??? Mapping/ (API-specific profiles using Application DTOs)
??? Helper/ (Response models)
??? Middleware/
??? wwwroot/
```

---

## Dependency Injection Flow

### In `Program.cs` (Souqna.API):
```csharp
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

// Register Infrastructure (which includes Application services)
builder.Services.AddInfrastructureServices(builder.Configuration);
```

### In `InfrastructureRegisteration.cs` (Souqna.Infrastructure):
```csharp
public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services, 
    IConfiguration configuration)
{
    // 1. Add Application services (AutoMapper, etc.)
    services.AddApplicationServices();

    // 2. Register Infrastructure implementations of Application interfaces
    services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepositories<>));
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IPhotoRepository, PhotoRepository>();
    services.AddSingleton<IImageManagementService, ImageManagementService>();

    // 3. Register DbContext
    services.AddDbContext<ApplicationDbContext>(options =>
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    });

    // 4. Register File Provider
    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
    if (!Directory.Exists(wwwrootPath))
    {
        Directory.CreateDirectory(wwwrootPath);
    }
    services.AddSingleton<IFileProvider>(new PhysicalFileProvider(wwwrootPath));

    return services;
}
```

---

## Project References

```
Souqna.API
    ? Souqna.Application
    ? Souqna.Infrastructure

Souqna.Application
    ? Souqna.Domin

Souqna.Infrastructure
    ? Souqna.Application
    ? Souqna.Domin

Souqna.Domin
    ? (no dependencies)
```

---

## What Files Moved Where

### From `Souqna.Domin` ? `Souqna.Application`
- ? `DTOs/CategoryDto.cs`
- ? `DTOs/ProductDto.cs`
- ? `Interfaces/IGenericRepository.cs`
- ? `Interfaces/ICategoryRepository.cs`
- ? `Interfaces/IProductRepository.cs`
- ? `Interfaces/IPhotoRepository.cs`
- ? `Interfaces/IUnitOfWork.cs`
- ? `Services/IImagemanagmentService.cs` ? `IImageManagementService.cs`

### Staying in `Souqna.Domin`
- ? `Entities/BaseEntity.cs`
- ? `Entities/Category.cs`
- ? `Entities/Product.cs`
- ? `Entities/Photo.cs`
- ? `Sharing/ProductParams.cs`

### New in `Souqna.Application`
- ? `ApplicationServiceRegistration.cs`
- ? `Mappings/CategoryMappingProfile.cs`
- ? `Mappings/ProductMappingProfile.cs`

---

## Step-by-Step Completion Checklist

### Step 1: Add Application Project to Solution
```bash
cd D:\Desktop\Souqna\Souqna
dotnet sln Souqna.sln add Souqna.Application\Souqna.Application.csproj
```

### Step 2: Clean Up Domain Project
Remove from `Souqna.Domin/` (they're now in Application):
- `DTOs/` folder
- `Interfaces/` folder
- `Services/` folder (keep domain services if any)

Update `Souqna.Domin.csproj` to have minimal dependencies:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Http.Features" Version="6.0.0-preview.4.21253.5" />
  </ItemGroup>
</Project>
```

### Step 3: Verify Project References

**Souqna.API.csproj:**
```xml
<ItemGroup>
  <ProjectReference Include="..\Souqna.Application\Souqna.Application.csproj" />
  <ProjectReference Include="..\Souqna.Infrastructure\Souqna.Infrastructure.csproj" />
</ItemGroup>
```

**Souqna.Infrastructure.csproj:**
```xml
<ItemGroup>
  <ProjectReference Include="..\Souqna.Domin\Souqna.Domin.csproj" />
  <ProjectReference Include="..\Souqna.Application\Souqna.Application.csproj" />
</ItemGroup>
```

**Souqna.Application.csproj:**
```xml
<ItemGroup>
  <ProjectReference Include="..\Souqna.Domin\Souqna.Domin.csproj" />
</ItemGroup>
```

### Step 4: Build and Test
```bash
dotnet clean
dotnet build
dotnet run --project Souqna.API
```

---

## Example: How ProductsController Uses All Layers

```csharp
// API Layer (Souqna.API/Controllers/ProductsController.cs)
using Souqna.API.Helper;
using Souqna.Application.DTOs;          // ? Application DTOs
using Souqna.Application.Interfaces.Repositories;  // ? Application Interfaces

[HttpGet]
public async Task<IActionResult> GetAllProducts([FromQuery] ProductParams productParams)
{
    try
    {
        // Call repository through interface
        var products = await unitOfWork.Products.GetAllAsync(productParams);
        // Returns Application DTOs
        var pagination = new Pagination<ProductDto>(products, ...);
        return Ok(pagination);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { Message = ex.Message });
    }
}

// Data Flow:
// 1. API receives request
// 2. API calls IUnitOfWork (Application interface)
// 3. Infrastructure implements IUnitOfWork ? ProductRepository
// 4. ProductRepository queries Domain entities (Product, Category)
// 5. ProductRepository maps Domain entities ? Application DTOs
// 6. API returns Application DTOs in response
```

---

## Important Notes

1. **Domain Layer is Pure**
   - No external dependencies (no EF, AutoMapper, etc.)
   - Contains only entities and business logic
   - Can be reused in different contexts

2. **Application Layer is the Contract**
   - Defines what Infrastructure must implement
   - Defines DTOs for data transfer
   - Isolated from presentation concerns

3. **Infrastructure Implements**
   - Provides concrete implementations of Application interfaces
   - Handles database access, file operations, external services
   - Should never be referenced directly by controllers

4. **API is the Entry Point**
   - Only references Application and Infrastructure
   - Uses Application DTOs and interfaces
   - Handles HTTP concerns (routing, status codes, etc.)

---

## Benefits of This Architecture

? **Testability**: Each layer can be tested independently
? **Maintainability**: Clear separation of concerns
? **Scalability**: Easy to add new features
? **Reusability**: Domain layer can be reused in other projects
? **Flexibility**: Easy to swap Infrastructure implementations
