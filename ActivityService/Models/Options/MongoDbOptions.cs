
using System.Collections.Generic;

namespace ActivityService.Models.Options
{
    public class MongoDbOptions
    {
        public string Hosts { get; set; }
        public string Database { get; set; }
        public ICollection<TypeCollectionName> EntityMappers { get; set; }
    }
}
