using KYC360.Models;

namespace KYC360.Services
{
    public interface IEntityService
    {
        Task<List<Entity>> GetAllAsync();
        Task<Entity?> GetByIdAsync(string id);
        Task CreateAsync(Entity entity);
        Task UpdateAsync(Entity entity);
        Task DeleteAsync(string id);
        Task<List<Entity>> GetEntitiesAsync(
            string search,
            string? gender,
            DateTime? startDate,
            DateTime? endDate,
            List<string> countries,
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "FirstName",
            string sortDirection = "asc");
    }
}