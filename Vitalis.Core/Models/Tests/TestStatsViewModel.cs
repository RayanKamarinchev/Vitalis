using IntelliTest.Core.Models.Questions;
using Vitalis.Core.Models.Questions;
using Vitalis.Core.Models.Questions.Closed;

namespace Vitalis.Core.Models.Tests
{
    public class TestStatsViewModel
    {
        public string Title { get; set; }
        public int TestTakers { get; set; }
        public List<ClosedQuestionStatsViewModel> ClosedQuestions { get; set; } =
            new List<ClosedQuestionStatsViewModel>();

        public List<OpenQuestionStatsViewModel> OpenQuestions { get; set; } =
            new List<OpenQuestionStatsViewModel>();

        public List<QuestionType> QuestionOrder;
        public float AverageScore { get; set; }
    }
}
