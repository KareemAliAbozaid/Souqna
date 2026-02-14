using Souqna.Domin.DTOs;
using Souqna.Domin.Entities;

namespace Souqna.Domin.Interfaces
{
    public interface IProductRepository: IGenericRepository< Product>
    {
        Task<bool> AddAsync(AddProductDto productDto);
        Task<bool> UpdateAsync(UpdateProductDto productDto);
        //Task DeleteAsync(Product product);
    }
}
