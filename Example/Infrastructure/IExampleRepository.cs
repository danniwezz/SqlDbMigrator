using Example.Models;

namespace Example.Infrastructure;

public interface IExampleRepository
{
    Task<ExampleEntity?> GetExampleEntityAsync(int id);
    Task<List<ExampleEntity>> GetExampleEntitiesAsync();
    Task AddExampleEntityAsync(ExampleEntity entity);
    Task UpdateExampleEntityAsync(ExampleEntity entity);
    Task DeleteExampleEntityAsync(int id);
}
