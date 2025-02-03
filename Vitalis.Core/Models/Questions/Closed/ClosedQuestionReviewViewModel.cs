namespace Vitalis.Core.Models.Questions.Closed
{
    public class ClosedQuestionReviewViewModel : ClosedQuestionViewModel
    {
        public bool[] CorrectAnswersArray { get; set; }
        public float Score { get; set; }
    }
}
