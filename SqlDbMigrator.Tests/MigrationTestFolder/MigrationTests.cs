using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;
using SqlDbMigrator.Migrator;

namespace SqlDbMigrator.Tests.MigrationTestFolder;
public class MigrationTests
{

    [Fact]
    public async Task Migrate()
    {
        var connectionString = "Server=localhost\\SQLSERVER;Initial Catalog=TestDb;Encrypt=False;Trusted_Connection=true;TrustServerCertificate=True;Connection Timeout=120;";
        var migrationType = typeof(MigrationAssemblyLocator);
        var migrator = new DbMigrator(new Logger<DbMigrator>(new NullLoggerFactory()));
        var act = async () => await migrator.Migrate(connectionString, migrationType);
        await act.ShouldNotThrowAsync();
    }

}
