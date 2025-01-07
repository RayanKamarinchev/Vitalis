namespace Vitalis.Core.Models.Questions.Closed
{
    public class ClosedQuestionReviewViewModel : ClosedQuestionViewModel
    {
        public int[] CorrectAnswers { get; set; }
        public float Score { get; set; }
    }
}
