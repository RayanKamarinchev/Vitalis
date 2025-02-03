using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class ClosedQuestion
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public IEnumerable<Answer> Answers { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public Guid TestId { get; set; }
        [ForeignKey(nameof(TestId))]
        public Test TestGroup { get; set; }
        public IEnumerable<ClosedQuestionAnswer> UserAnswers { get; set; }
        public float MaxScore { get; set; }
        public string ImagePath { get; set; }
    }
}
