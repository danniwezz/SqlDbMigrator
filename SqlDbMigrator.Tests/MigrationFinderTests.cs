using SqlDbMigrator.Tests.MigrationTestFolder;
using System.Reflection;

namespace SqlDbMigrator.Tests;

public class MigrationFinderTests
{
    [Fact]
    public void GetMigrationsList()
    {
        var expectedVersion = 1;
        var expectedMigrationName = "TestDbMigrationScript";
        var expectedFileName = "000_000_000_001_TestDbMigrationScript.sql";

        var migrations = MigrationFinder.Find(typeof(MigrationAssemblyLocator));


        Assert.Single(migrations);
        var migration = migrations.Single();
        Assert.Equal(expectedVersion, migration.Version);
        Assert.Equal(expectedMigrationName, migration.Name);
        Assert.Equal(expectedFileName, Path.GetFileName(migration.FilePath));
    }

    [Fact]
    public void MigrationFolderDoesNotHaveDuplicateVersions()
    {
         MigrationFinder.Find(typeof(MigrationAssemblyLocator));
    }
}