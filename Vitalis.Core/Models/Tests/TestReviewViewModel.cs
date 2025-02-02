using Vitalis.Core.Models.Questions;
using Vitalis.Core.Models.Questions.Closed;
using Vitalis.Core.Models.Questions.Open;

namespace Vitalis.Core.Models.Tests
{
    public class TestReviewViewModel
    {
        public List<ClosedQuestionReviewViewModel> ClosedQuestions { get; set; }
        public List<OpenQuestionReviewViewModel> OpenQuestions { get; set; }
        public List<QuestionType> QuestionsOrder { get; set; }
        public float Score { get; set; }
    }
}
