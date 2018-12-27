using System.Collections.Generic;
using MongoDB.Driver;

namespace ActivityService.Models
{
    public class EntityMapperOptions
    {
        public ICollection<TypeCollectionName> Collection { get; set; }
    }
}