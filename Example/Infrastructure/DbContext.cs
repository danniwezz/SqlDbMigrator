using Example.Models;
using Microsoft.EntityFrameworkCore;

namespace Example.Infrastructure;

public class ExampleDbContext : DbContext
{
    public ExampleDbContext(DbContextOptions<ExampleDbContext> options) : base(options)
    {
    }

    public DbSet<ExampleEntity> Example { get; set; }
}
