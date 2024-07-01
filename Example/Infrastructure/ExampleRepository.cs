using Example.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure;

public class ExampleRepository : IExampleRepository
{
    private readonly ExampleDbContext _dbContext;

    public ExampleRepository(ExampleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ExampleEntity?> GetExampleEntityAsync(int id)
    {
        return await _dbContext.Example.FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<ExampleEntity>> GetExampleEntitiesAsync()
    {
        return await _dbContext.Example.ToListAsync();
    }

    public async Task AddExampleEntityAsync(ExampleEntity entity)
    {
        await _dbContext.Example.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateExampleEntityAsync(ExampleEntity entity)
    {
        _dbContext.Example.Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteExampleEntityAsync(int id)
    {
        var entity = await GetExampleEntityAsync(id);
        if(entity == null)
        {
            return;
        }
        _dbContext.Example.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}