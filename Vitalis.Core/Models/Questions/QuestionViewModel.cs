using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Vitalis.Core.Models.Questions
{
    public class QuestionViewModel
    {
        [Required]
        public string Text { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Id { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ImagePath { get; set; }
        public int Index { get; set; }
        public IFormFile? Image { get; set; }
    }
}
