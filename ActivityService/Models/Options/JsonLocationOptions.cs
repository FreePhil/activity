namespace ActivityService.Models.Options
{
    public class JsonLocationOptions
    {
        public string EduSubjectFilename { get; set; }
        public string EduSubjectUri { get; set; }
        
        public string TestGoSubjectFilename { get; set; }
        public string TestGoSubjectUri { get; set; }
        public string TestGoPermissibleUri { get; set; }
        public CacheConstantName CacheName { get; set; }
    }
}