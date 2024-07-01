using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace SqlDbMigrator.Migrator
{

    public class DbMigrator : IDbMigrator
    {
        private readonly ILogger<DbMigrator> _logger;
        private List<Migration> _migrations = new List<Migration>();

        public DbMigrator(ILogger<DbMigrator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Searches for all migrations in the given assembly and executes them in the given database.
        /// </summary>
        /// <param name="connectionString">The connectionString to the database where the migration should be performed.</param>
        /// <param name="migrationAssembly">The assembly of the location of the migrations.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task Migrate(string connectionString, Type migrationType)
        {
            await CreateIfDatabaseNotExists(connectionString);

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection == null)
                {
                    throw new Exception("Connection must be initialized");
                }
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                var transaction = connection.BeginTransaction();

                await CreateVersionTableIfNotExists(transaction);

                var migrations = MigrationFinder.Find(migrationType);

                await ExecuteMigrations(transaction, migrations);

                transaction.Commit();
                foreach (var migration in _migrations)
                {
                    _logger.LogInformation($"Migration {migration.Name} with version {migration.Version} executed successfully.");
                }
            };
        }

        private async Task CreateVersionTableIfNotExists(IDbTransaction transaction)
        {
            var tableName = "Version";
            var query = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            if (!(await transaction.Connection.QuerySingleAsync<int>(query, new { tableName = tableName }, transaction) > 0))
            {
                var createTableQuery = $@"
                                    CREATE TABLE [{tableName}]
                                    (
                                    [Version] BIGINT NOT NULL,
                                    MigrationName NVARCHAR(255) NOT NULL,
                                    FileName NVARCHAR(255) NOT NULL,
                                    CreatedAt DATETIME NOT NULL CONSTRAINT [DF_Version_CreatedAt] DEFAULT GETDATE(),
                                    CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED ([Version] ASC)
                                    )
                                    ";
                await transaction.Connection.ExecuteAsync(createTableQuery, transaction: transaction);
            }

        }

        private async Task ExecuteMigrations(IDbTransaction transaction, List<Migration> migrations)
        {
            var latestVersion = await transaction.Connection.QuerySingleAsync<long>("SELECT COALESCE(MAX([Version]),0) FROM [Version]", transaction: transaction);
            foreach (var migration in migrations.Where(x => x.Version > latestVersion).OrderBy(x => x.Version))
            {
                await ExecuteMigration(transaction, migration);
            }
        }

        private async Task ExecuteMigration(IDbTransaction transaction, Migration migration)
        {
            try
            {
                var sqlQuery = File.ReadAllText(migration.FilePath);
                await transaction.Connection.ExecuteAsync(sqlQuery, transaction: transaction);
                await transaction.Connection.ExecuteAsync($"INSERT INTO [Version] ([Version], MigrationName, FileName) VALUES (@Version, @MigrationName, @FileName)",
                    new { Version = migration.Version, MigrationName = migration.Name, FileName = migration.FileName}, transaction: transaction);
                _migrations.Add(migration);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error executing migration {migration.Name}");
                throw;
            }
        }

        private async Task CreateIfDatabaseNotExists(string connectionString)
        {
            using(var connection = new SqlConnection(GetConnectionstringWithoutCatalog(connectionString)))
            {

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                var databaseName = GetDatabaseNameFromConnectionString(connectionString);
                var checkIfDatabaseExistsQuery = @"
                                            IF (EXISTS (SELECT name 
                                            FROM master.sys.databases 
                                            WHERE ('[' + name + ']' = @dbName 
                                            OR name = @dbName)))
                                            BEGIN
                                            SELECT 1
                                            END
                                            ELSE
                                            BEGIN
                                            SELECT 0
                                            END
                                            ";
                if (!await connection.QuerySingleAsync<bool>(checkIfDatabaseExistsQuery, new { dbName = databaseName }))
                {
                    _logger.LogInformation($"Database {databaseName} does not exist. Creating database...");
                    await connection.ExecuteAsync($"CREATE DATABASE {databaseName}");
                    _logger.LogInformation($"Database {databaseName} created successfully.");
                }
            }
        }

        private string GetConnectionstringWithoutCatalog(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = "";
            return builder.ConnectionString;
        }

        private string GetDatabaseNameFromConnectionString(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("ConnectionString is empty or null");
            }
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }
    }
}
