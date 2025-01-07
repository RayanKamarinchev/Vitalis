namespace Vitalis.Core.Models.Questions.Closed
{
    public class ClosedQuestionStatsViewModel
    {
        public string[] Answers { get; set; }
        public string Text { get; set; }
        public List<List<int>> StudentAnswers { get; set; }
        public string ImagePath { get; set; }
    }
}
