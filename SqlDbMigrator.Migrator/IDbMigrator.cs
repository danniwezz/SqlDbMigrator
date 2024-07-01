namespace SqlDbMigrator.Migrator
{
    public interface IDbMigrator
    {
       public Task Migrate(string connectionString, Type migrationType);
    }
}