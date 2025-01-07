using System.ComponentModel.DataAnnotations;
using Vitalis.Core.Models.Questions;
using Vitalis.Core.Models.Questions.Closed;
using Vitalis.Core.Models.Questions.Open;

namespace Vitalis.Core.Models.Tests
{
    public class TestEditViewModel
    {
        public List<bool> Groups { get; set; }
        public bool IsPublic { get; set; }
        public List<OpenQuestionViewModel>? OpenQuestions { get; set; }
        public List<ClosedQuestionViewModel>? ClosedQuestions { get; set; }
        public List<QuestionType>? QuestionsOrder { get; set; }
        public string Title { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; }
        public int Grade { get; set; }
        public Guid? Id { get; set; }
    }
}
