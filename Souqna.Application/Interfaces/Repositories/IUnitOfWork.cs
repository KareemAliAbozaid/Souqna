namespace Souqna.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IPhotoRepository Photos { get; }
        Task<bool> SaveChangesAsync();
    }
}
