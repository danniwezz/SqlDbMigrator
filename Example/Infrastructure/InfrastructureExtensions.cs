using SqlDbMigrator.Migrator;
using Example.Migrations;

namespace Example.Infrastructure
{
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
}