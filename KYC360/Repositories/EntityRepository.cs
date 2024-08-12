using KYC360.Database;
using KYC360.Models;

namespace KYC360.Repositories
{
    public class EntityRepository : IEntityRepository
    {
        private readonly ILogger<EntityRepository> _logger;
        // Parameters for retry and backoff mechanism
        private const int MaxRetryAttempts = 3;
        private const int InitialDelayMs = 500; // Initial delay in milliseconds
        private const double BackoffMultiplier = 2.0; // Exponential backoff multiplier

        public EntityRepository(ILogger<EntityRepository> logger)
        {
            _logger = logger;
        }

        public Task<List<Entity>> GetAllAsync()
        {
            return Task.FromResult(MockDatabase.Entities);
        }

        public Task<Entity?> GetByIdAsync(string id)
        {
            Entity? entity = MockDatabase.Entities.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(entity);
        }

        public Task AddAsync(Entity entity)
        {
            return RetryAsync(() =>
            {
                MockDatabase.Entities.Add(entity);
                return Task.CompletedTask;
            });
        }

        public Task UpdateAsync(Entity entity)
        {
            return RetryAsync(() =>
            {
                Entity? existingEntity = MockDatabase.Entities.FirstOrDefault(e => e.Id == entity.Id);
                if (existingEntity != null)
                {
                    MockDatabase.Entities.Remove(existingEntity);
                    MockDatabase.Entities.Add(entity);
                }
                return Task.CompletedTask;
            });
        }

        public Task DeleteAsync(string id)
        {
            return RetryAsync(() =>
            {
                Entity? entity = MockDatabase.Entities.FirstOrDefault(e => e.Id == id);
                if (entity != null)
                {
                    MockDatabase.Entities.Remove(entity);
                }
                return Task.CompletedTask;
            });
        }

         // Retry and Backoff mechanism for database write operations
        private async Task RetryAsync(Func<Task> operation)
        {
            int attempt = 0;
            int delayMs = InitialDelayMs;

            while (true)
            {
                try
                {
                    attempt++;
                    await operation();
                    _logger.LogInformation("Operation succeeded on attempt {Attempt}.", attempt);
                    break; // If operation succeeds, exit the loop
                }
                catch (Exception ex) when (attempt < MaxRetryAttempts)
                {
                    // Log the error
                    _logger.LogWarning("Operation failed on attempt {Attempt}. Retrying in {Delay}ms... Error: {Error}", attempt, delayMs, ex.Message);

                    // Wait for the delay period before retrying
                    await Task.Delay(delayMs);

                    // Apply exponential backoff
                    delayMs = (int)(delayMs * BackoffMultiplier);
                }
                catch (Exception ex)
                {
                    // If the max number of attempts is reached, rethrow the exception
                    _logger.LogError("Operation failed after {MaxAttempts} attempts. Error: {Error}", MaxRetryAttempts, ex.Message);
                    throw;
                }
            }
        }
    }
}