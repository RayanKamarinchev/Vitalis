namespace Vitalis.Core.Models.Questions.Closed
{
    public class ClosedQuestionViewModel : QuestionViewModel
    {
        public string[] Options { get; set; }
        public bool[] UsersAnswersArray { get; set; }
        public float MaxScore { get; set; }
    }
}
