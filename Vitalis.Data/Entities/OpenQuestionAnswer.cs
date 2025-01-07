using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class OpenQuestionAnswer
    {
        [Key]
        public Guid Id { get; set; }
        
        public string? Answer { get; set; }
        public OpenQuestion Question { get; set; }
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public float Points { get; set; }
        public string? Explanation { get; set; }
    }
}
