using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class OpenQuestion
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public string Answer { get; set; }
        public bool IsDeleted { get; set; }
        [Required]
        public Guid TestId { get; set; }
        [ForeignKey(nameof(TestId))]
        public Test Test { get; set; }

        public IEnumerable<OpenQuestionAnswer> UserAnswers { get; set; }
        public float MaxScore { get; set; }

        public string ImagePath { get; set; }
    }
}
