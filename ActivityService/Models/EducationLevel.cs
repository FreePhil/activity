using System.Collections.Generic;

namespace ActivityService.Models
{
    public class EducationLevel
    {
        public int Id { get; set; }
        public string SchoolType { get; set; }
        private IList<Subject> Subjects { get; set; }
    }
}