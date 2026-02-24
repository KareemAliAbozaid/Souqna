using Souqna.Domin.DTOs;
using Souqna.Domin.Entities;
using Souqna.Domin.Sharing;

namespace Souqna.Domin.Interfaces
{
    public interface IProductRepository: IGenericRepository< Product>
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(ProductParams productParams);
        Task<bool> AddAsync(AddProductDto productDto);
        Task<bool> UpdateAsync(UpdateProductDto productDto);
    }
}
