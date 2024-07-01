# SqlDbMigrator

SqlDbMigrator is a tool designed to simplify database migrations using a database-first approach. This project aims to streamline development by managing versioned database migrations effectively.

### Key Features

- **Database Initialization**: Automatically creates the database if it doesn't exist.
- **Version Tracking**: Maintains a version table to keep track of executed migrations.
- **Transactional Migrations**: Ensures that migrations are executed within transactions to maintain database integrity. If any migration fails, changes are rolled back.

Currently supports SQL Server.

## Solution

The package is designed to run on application startup.

1. **Database Check and Creation**: The connection string is checked for the database name. If the database doesn't exist, it will be created.
2. **Version Table Creation**: A version table is created in the database to store the executed versions.
3. **Migration Execution**: The migrator scans the migration directory, extracts the versions, and compares them with the version table in the database. Migrations that haven't been executed will run in a transaction.
    - If any migration fails, the transaction will be rolled back, and no migrations will be executed. An exception will be thrown.
    - If all migrations succeed, the transaction will be committed.

## Setup

1. Install the NuGet package in your project.
2. Create a folder for migrations.
3. Create a SQL file in the following format: `XXX_XXX_XXX_XXX_NameOfMigration.sql`.
4. Write your migration.
5. [Set up the migrator](#extension-method).
6. [Use the migrator on startup](#usage).
7. Run the application.

## Example

[An example project demonstrates how to use the package can be found here](https://github.com/danniwezz/SqlDbMigrator/tree/main/Example)

### Extension Method
Example: [InfrastructureExtensions.cs](https://github.com/danniwezz/SqlDbMigrator/blob/main/Example/Infrastructure/InfrastructureExtensions.cs)
```csharp
public static class InfrastructureExtensions
{
    public static IServiceCollection AddDbMigrator(this IServiceCollection services)
    {
        services.AddScoped<IDbMigrator, DbMigrator>();
        return services;
    }

    public static IApplicationBuilder UseDbMigrator(this IApplicationBuilder app, string connectionString)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<IDbMigrator>();
        migrator.Migrate(connectionString, typeof(MigrationAssemblyLocator));
        return app;
    }
}
```

# Usage

In the startup class (e.g., [Program.cs](https://github.com/danniwezz/SqlDbMigrator/blob/main/Example/Program.cs)):

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? throw new Exception("Default SQL connection string is null");

// Add DbMigrator to the dependency injection container
services.AddDbMigrator();

var app = builder.Build();

// Use DbMigrator to migrate the database
app.UseDbMigrator(connectionString);
```

By following these steps, you can ensure your database is always up to date with the latest migrations upon application startup.