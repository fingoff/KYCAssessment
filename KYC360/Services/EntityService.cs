using KYC360.Models;
using KYC360.Repositories;

namespace KYC360.Services
{
    public class EntityService : IEntityService
    {
        private readonly IEntityRepository _repository;

        public EntityService(IEntityRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Entity>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Entity?> GetByIdAsync(string id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task CreateAsync(Entity entity)
        {
            await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(Entity entity)
        {
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<List<Entity>> GetEntitiesAsync(
            string search,
            string? gender,
            DateTime? startDate,
            DateTime? endDate,
            List<string> countries,
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "FirstName",
            string sortDirection = "asc")
        {
            var entities = await _repository.GetAllAsync();

            // Apply search string logic
            entities = ApplySearchFilter(entities, search);

            // Apply advanced filtering logic
            entities = ApplyFilters(entities, gender, startDate, endDate, countries);

            // Apply sorting
            entities = ApplySorting(entities, sortBy, sortDirection);

            // Apply pagination
            entities = ApplyPagination(entities, pageNumber, pageSize);

            return entities;
        }

        private static List<Entity> ApplySearchFilter(List<Entity> entities, string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return entities;

            var searchTerms = search.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            entities = entities.Where(e =>
                searchTerms.All(term =>
                    e.Names.Any(n => 
                        (n.FirstName != null && n.FirstName.Contains(term, StringComparison.CurrentCultureIgnoreCase)) ||
                        (n.MiddleName != null && n.MiddleName.Contains(term, StringComparison.CurrentCultureIgnoreCase)) ||
                        (n.Surname != null && n.Surname.Contains(term, StringComparison.CurrentCultureIgnoreCase))
                    ) ||
                    e.Addresses.Any(a =>
                        (a.AddressLine != null && a.AddressLine.Contains(term, StringComparison.CurrentCultureIgnoreCase)) ||
                        (a.Country != null && a.Country.Contains(term, StringComparison.CurrentCultureIgnoreCase))
                    )
                )
            ).ToList();

            // if (!string.IsNullOrWhiteSpace(search))
            // {
            //     search = search.ToLower();
            //     entities = entities.Where(e =>
            //         e.Names.Any(n => n.FirstName != null && n.FirstName.Contains(search, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Names.Any(n => n.MiddleName != null && n.MiddleName.Contains(search, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Names.Any(n => n.Surname != null && n.Surname.Contains(search, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Addresses.Any(a => a.AddressLine != null && a.AddressLine.Contains(search, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Addresses.Any(a => a.Country != null && a.Country.Contains(search, StringComparison.CurrentCultureIgnoreCase))
            //     ).ToList();
            // }

            // if (!string.IsNullOrWhiteSpace(search))
            // {
            //     search = search.ToLower();
            //     entities = entities.Where(e =>
            //         e.Names.Any(n => n.FirstName != null && search.Contains(n.FirstName, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Names.Any(n => n.MiddleName != null && search.Contains(n.MiddleName, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Names.Any(n => n.Surname != null && search.Contains(n.Surname, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Addresses.Any(a => a.AddressLine != null && search.Contains(a.AddressLine, StringComparison.CurrentCultureIgnoreCase)) ||
            //         e.Addresses.Any(a => a.Country != null && search.Contains(a.Country, StringComparison.CurrentCultureIgnoreCase))
            //     ).ToList();
            // }

            return entities;
        }

        private static List<Entity> ApplyFilters(List<Entity> entities, string? gender, DateTime? startDate, DateTime? endDate, List<string> countries)
        {
            // Apply gender filter
            if (!string.IsNullOrWhiteSpace(gender))
            {
                entities = entities.Where(e => e.Gender != null && e.Gender.Equals(gender, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }

            // Apply startDate filter
            if (startDate != null)
            {
                entities = entities.Where(e => e.Dates.Any(d => d.Value != null && d.Value >= startDate)).ToList();
            }

            // Apply endDate filter
            if (endDate != null)
            {
                entities = entities.Where(e => e.Dates.Any(d => d.Value != null && d.Value <= endDate)).ToList();
            }

            // Apply countries filter
            if (countries != null && countries.Count > 0)
            {
                entities = entities.Where(e => e.Addresses.Any(a => a.Country != null && countries.Contains(a.Country))).ToList();
            }

            return entities;
        }

        private static List<Entity> ApplySorting(List<Entity> entities, string sortBy, string sortDirection)
        {
            // Default sorting to FirstName if the provided sortBy is invalid
            sortBy = string.IsNullOrWhiteSpace(sortBy) ? "FirstName" : sortBy;

            // Apply sorting based on the sortBy parameter
            entities = sortBy switch
            {
                "FirstName" => sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? entities.OrderByDescending(e => e.Names.First().FirstName).ToList()
                    : entities.OrderBy(e => e.Names.First().FirstName).ToList(),
                
                "Surname" => sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? entities.OrderByDescending(e => e.Names.First().Surname).ToList()
                    : entities.OrderBy(e => e.Names.First().Surname).ToList(),
                
                "Dates" => sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? entities.OrderByDescending(e => e.Dates.First().Value).ToList()
                    : entities.OrderBy(e => e.Dates.First().Value).ToList(),

                _ => entities.OrderBy(e => e.Names.First().FirstName).ToList(),
            };

            return entities;
        }

        private static List<Entity> ApplyPagination(List<Entity> entities, int pageNumber, int pageSize)
        {
            pageSize = Math.Min(pageSize, 100); // Ensure max page size limit
            pageNumber = Math.Max(pageNumber, 1); // Ensure page number is at least 1

            return entities.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }
    }
}