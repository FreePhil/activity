using System;
using System.Collections.Generic;

namespace ActivityService.Models.Options
{
    public class MongoEntityOptions
    {
        public IList<TypeCollectionName> Mappers { get; set; }   
    }
}