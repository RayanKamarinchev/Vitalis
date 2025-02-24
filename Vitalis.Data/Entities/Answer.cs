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

        public Guid ClosedQuestionId { get; set; }
        [ForeignKey(nameof(ClosedQuestionId))]
        public ClosedQuestion ClosedQuestion { get; set; }

        public int Order { get; set; }
    }
}
