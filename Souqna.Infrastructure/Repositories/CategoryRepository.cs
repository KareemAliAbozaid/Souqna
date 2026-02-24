using Souqna.Domin.Entities;
using Souqna.Application.Interfaces.Repositories;
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
