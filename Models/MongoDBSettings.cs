namespace MongoExample.Models;

/*-----------------------------------------------------------------
This class is used to store settings for connecting to our MongoDB
database. 

-------------------------------------------------------------------*/

public class MongoDBSettings
{
    public string ConnectionURI { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string CollectionName { get; set; } = null!;
}