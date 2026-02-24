using Souqna.Domin.Entities;
using Souqna.Application.Interfaces.Repositories;
using Souqna.Infrastructure.Data;

namespace Souqna.Infrastructure.Repositories
{
    public class PhotoRepository : GenericRepositories<Photo>, IPhotoRepository
    {
        public PhotoRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
