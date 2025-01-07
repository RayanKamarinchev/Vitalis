using System.ComponentModel.DataAnnotations;
using Vitalis.Core.Models.Questions;
using Vitalis.Core.Models.Questions.Closed;
using Vitalis.Core.Models.Questions.Open;

namespace Vitalis.Core.Models.Tests
{
    public class TestSubmitViewModel
    {
        public List<OpenQuestionSubmitViewModel> OpenQuestions { get; set; }
        public List<ClosedQuestionViewModel> ClosedQuestions { get; set; }
        public List<QuestionType> QuestionOrder;
        [Display(Name = "Заглавие: ")]
        public string Title { get; set; }
        public Guid Id { get; set; }
    }
}
