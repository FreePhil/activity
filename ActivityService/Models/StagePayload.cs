namespace ActivityService.Models
{
    public class StagePayload
    {
        public string Name { get; set; }
        public string Payload { get; set; }
        
        public StagePayload History { get; set; } = null;
    }
}