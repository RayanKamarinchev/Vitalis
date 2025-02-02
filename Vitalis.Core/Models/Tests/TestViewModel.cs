using System.ComponentModel.DataAnnotations;
using Vitalis.Core.Infrastructure;

namespace Vitalis.Core.Models.Tests
{
    public class TestViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [StringLength(Constants.TestTitleMaxLength, MinimumLength = Constants.TestTitleMinLength)]
        public string Title { get; set; }
        [Required]
        public List<string> Groups { get; set; }
        [Required]
        [Range(1, 12)]
        public int Grade { get; set; }
        [Required]
        public bool IsPublic { get; set; }
        [Required]
        [StringLength(Constants.DescriptionMaxLength)]
        public string Description { get; set; }
        public string School { get; set; }
        [Required]
        public string Author { get; set; }
        public float AverageScore { get; set; }
        public int TestTakers { get; set; }
        public string CreatedOn { get; set; }
        public bool IsCreator { get; set; }
        public bool IsTestTaken { get; set; }
        public int QuestionsCount { get; set; }
    }
}