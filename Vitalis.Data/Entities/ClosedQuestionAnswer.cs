using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class ClosedQuestionAnswer
    {
        [Key]
        public Guid Id { get; set; }
        public string? AnswerIndexes { get; set; }
        public ClosedQuestion Question { get; set; }
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public decimal Points { get; set; }
        public string? Explanation { get; set; }
    }
}
