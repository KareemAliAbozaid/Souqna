using Souqna.Domin.Entities;
using Souqna.Domin.Interfaces;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class CategoryRepository : GenericRepositories<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
