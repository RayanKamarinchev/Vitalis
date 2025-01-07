using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class TestLike
    {
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public Guid TestId { get; set; }
        [ForeignKey(nameof(TestId))]
        public Test Test { get; set; }
    }
}
