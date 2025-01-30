namespace Vitalis.Core.Models.Questions.Closed
{
    public class ClosedQuestionViewModel : QuestionViewModel
    {
        public string[] Options { get; set; }
        public bool[] AnswerIndexes { get; set; }
        public float MaxScore { get; set; }
    }
}
