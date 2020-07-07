using System.Collections.Generic;

namespace ActivityService.Models
{
    public class Product: LookupModel
    {
        public IList<LookupModel> Versions { get; set; } = new List<LookupModel>();
    }
}