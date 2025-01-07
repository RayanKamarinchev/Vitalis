using System.ComponentModel.DataAnnotations;
using Vitalis.Data.Entities;

namespace Vitalis.Core.Models.Tests
{
    public class RawTestViewModel
    {
        public Guid Id { get; set; }
        public string QuestionsOrder { get; set; }
        public List<OpenQuestion> OpenQuestions { get; set; } = new List<OpenQuestion>();
        public List<ClosedQuestion> ClosedQuestions { get; set; } = new List<ClosedQuestion>();
        public string TestTitle { get; set; }
    }
}
