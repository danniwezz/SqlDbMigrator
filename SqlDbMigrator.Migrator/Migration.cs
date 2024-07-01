namespace SqlDbMigrator
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="MigrationName">The name of the migration</param>
    /// <param name="FilePath">The absolute path of the file for the migration</param>
    /// <param name="Version">The version of the migration</param>
    public class Migration
    {
        public Migration(string name, string fileName, string filePath, long version)
        {
            Name = name;
            FileName = fileName;
            FilePath = filePath;
            Version = version;
        }
            
        public string Name { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public long Version { get; set; }
    }
}
