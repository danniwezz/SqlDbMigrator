using Example;
using Example.Infrastructure;
using Example.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddScoped<IExampleRepository, ExampleRepository>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new Exception("Default sql connection string is null");


services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


//Add DbMigrator to dependency injection container
services.AddDbMigrator();
services.AddDbContext<ExampleDbContext>(
        options => options.UseSqlServer(connectionString));


var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Use DbMigrator to migrate the database
app.UseDbMigrator(connectionString);

app.UseHttpsRedirection();

var group = app.MapGroup("example");
group.MapGet("/", (IExampleRepository exampleRepository) => exampleRepository.GetExampleEntitiesAsync());
group.MapPost("/", (IExampleRepository exampleRepository, ExampleEntity entity) => exampleRepository.AddExampleEntityAsync(entity));
group.MapDelete("/{id}", (IExampleRepository exampleRepository, int id) => exampleRepository.DeleteExampleEntityAsync(id));


app.Run();

