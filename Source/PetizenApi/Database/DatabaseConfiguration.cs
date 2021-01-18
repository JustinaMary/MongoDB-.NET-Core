namespace PetizenApi.Database
{
    public class DatabaseConfiguration
    {
        public string DefaultConnection { get; set; }
    }

    public class SQLConfiguration
    {
        public string ConnectionString { get; set; }
    }
    public class MongoSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
    }
}
