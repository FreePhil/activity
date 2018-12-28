﻿using System.Collections.Generic;
using ActivityService.Models;

namespace ActivityService.Repositories
{
    public class MongoDbOptions
    {
        public string Hosts { get; set; }
        public string Database { get; set; }
    }
}
