using System.Collections.Generic;

namespace ActivityService.Models
{
    public class Product: LookupModel
    {
        public IList<LookupModel> Verions { get; set; }
    }
}