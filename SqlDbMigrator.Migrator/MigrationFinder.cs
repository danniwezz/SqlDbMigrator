namespace SqlDbMigrator;
public static class MigrationFinder
{
    /// <summary>
    /// Locates all sql files in the given assembly and returns them as a list of migrations.
    /// </summary>
    /// <param name="migrationAssembly">The assembly of the migration folder.</param>
    /// <returns></returns>
    public static List<Migration> Find(Type migrationType)
    {
        var list = new List<Migration>();
        // Get the directory path of the assembly
        string assemblyDirectory = GetAssemblyDirectory(migrationType);

        // Search for .sql files in the directory
        var sqlFiles = Directory.GetFiles(assemblyDirectory, "*.sql", SearchOption.AllDirectories);

        foreach (var sqlFile in sqlFiles.Where(x => x.EndsWith(".sql")))
        {
            //Get file name
            var file = new FileInfo(sqlFile);
            var version = ExtractVersion(file.Name);
            var migrationName = file.Name.Replace(file.Extension, "").Replace(GetVersionFromFileName(file.Name), "").Trim('_');
            list.Add(new Migration(migrationName, file.Name, file.FullName, version));
        }
        CheckIfNoDuplicateMigrationExists(list);
        return list;
    }

    private static string GetVersionFromFileName(string fileName)
    {
        return fileName.Substring(0, 15);
    }

    private static long ExtractVersion(string fileName)
    {
        if(long.TryParse(GetVersionFromFileName(fileName).Replace("_", ""), out var version))
        {
            return version;
        }
        throw new Exception("Invalid version format. Format needs to be in 'XXX_XXX_XXX_XXX_NameOfMigration.sql'");
    }

    private static string GetAssemblyDirectory(Type type)
    {
        return Path.GetDirectoryName(type.Assembly.Location);
    }

    public static void CheckIfNoDuplicateMigrationExists(List<Migration> migrations)
    {
        var duplicateMigrations = migrations.GroupBy(x => x.Version).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        if (duplicateMigrations.Any())
        {
            throw new Exception($"Duplicate migration versions found: {string.Join(", ", duplicateMigrations)}");
        }
    }
}