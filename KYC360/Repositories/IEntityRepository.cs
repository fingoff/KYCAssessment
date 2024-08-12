using KYC360.Models;

namespace KYC360.Repositories
{
    public interface IEntityRepository
    {
        Task<List<Entity>> GetAllAsync();
        Task<Entity?> GetByIdAsync(string id);
        Task AddAsync(Entity entity);
        Task UpdateAsync(Entity entity);
        Task DeleteAsync(string id);
    }
}