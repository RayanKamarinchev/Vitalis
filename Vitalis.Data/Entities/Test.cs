using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class Test
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(DataConstants.TestTitleMaxLength)]
        public string Title { get; set; }
        [Required]
        public List<TestOrganicGroup> Groups { get; set; }
        [Required]
        [Range(0, 12)]
        public int Grade { get; set; }
        [Required]
        [StringLength(DataConstants.TestDescriptionMaxLength)]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public User Creator { get; set; }
        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; }
        public bool IsPublic { get; set; }
        public IEnumerable<TestLike> TestLikes { get; set; }
        public string? QuestionsOrder { get; set; }
        public List<OpenQuestion> OpenQuestions { get; set; } = new List<OpenQuestion>();
        public List<ClosedQuestion> ClosedQuestions { get; set; } = new List<ClosedQuestion>();
        public IEnumerable<TestResult> TestResults { get; set; }
    }
}
