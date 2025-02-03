using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class Answer
    {
        [Key]
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public Guid QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public ClosedQuestion Question { get; set; }
    }
}
