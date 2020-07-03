using System.Collections.Generic;
using Newtonsoft.Json;

namespace ActivityService.Models
{
    public class EducationLevel
    {
        public int Id { get; set; }
        public string SchoolType { get; set; }
        public IList<Subject> Subjects { get; set; } = new List<Subject>();
    }
}