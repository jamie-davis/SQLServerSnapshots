using DbTests.TestConfiguration;
using SQLServerSnapshots;

namespace DbTests
{
    public class SchemaObjectMother
    {
        public static SqlSnapshotCollection MakeCollection()
        {
            var collection = new SqlSnapshotCollection(Config.ConnectionString);
            collection.ConfigureSchema("Chess"); //this loads table definitions direct from the database schema. In theory no further config should be required
            collection.LoadSchemaOverrides(typeof(SchemaObjectMother).Assembly);

            //In practice, however, some extra configuration settings are needed
            return collection;
        }
    }
}