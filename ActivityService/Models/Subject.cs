using System.Collections.Generic;

namespace ActivityService.Models
{
    public class Subject: LookupModel
    {
        public bool HasBadge { get; set; } = false;
        public string ImageUrl { get; set; } = null;
        public IList<Product> Products { get; set; } = new List<Product>();
    }
}