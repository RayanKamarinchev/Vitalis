namespace Vitalis.Core.Models.Questions.Open
{
    public class OpenQuestionReviewViewModel : OpenQuestionViewModel
    {
        public string CorrectAnswer { get; set; }
        public float Score { get; set; }
        public string Explanation { get; set; }
    }
}
