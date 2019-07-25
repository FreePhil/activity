namespace ActivityService.Models
{
    public class PatternItem
    {
        public string QuestionType { get; set; }
        public int QuestionNumber { get; set; }
        public int AnsweringNumber { get; set; }
        public bool IsCrossDomain { get; set; } = false;
        public string Level { get; set; }
    }
}