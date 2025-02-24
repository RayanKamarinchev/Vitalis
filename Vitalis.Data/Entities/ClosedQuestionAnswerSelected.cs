using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class ClosedQuestionAnswerSelected
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(ClosedQuestionAnswerId))]
        public ClosedQuestionAnswer ClosedQuestionAnswer { get; set; }
        public Guid ClosedQuestionAnswerId { get; set; }
        public int AnswerIndex { get; set; }
    }
}
