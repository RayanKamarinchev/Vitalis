using System.ComponentModel.DataAnnotations;
using Vitalis.Core.Infrastructure;

namespace Vitalis.Core.Models.Tests
{
    public class TestCreateViewModel
    {
        [Required]
        [StringLength(Constants.TestTitleMaxLength, MinimumLength = Constants.TestTitleMinLength)]
        public string Title { get; set; }
        [Required]
        [Length(12, 12)]
        public bool[] Groups { get; set; }
        [Required]
        [Range(1, 12)]
        public int Grade { get; set; }
        [Required]
        public bool IsPublic { get; set; }
        [Required]
        [StringLength(Constants.DescriptionMaxLength)]
        public string Description { get; set; }
    }
}
