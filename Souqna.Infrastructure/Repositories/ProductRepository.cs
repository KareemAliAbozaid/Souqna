using Souqna.Domin.Entities;
using Souqna.Domin.Interfaces;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepositories<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
