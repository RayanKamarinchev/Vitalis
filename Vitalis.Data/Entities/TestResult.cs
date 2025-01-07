using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class TestResult
    {
        public Guid TestId { get; set; }
        [ForeignKey(nameof(TestId))]
        public Test Test { get; set; }
        public string TestTakerId { get; set; }
        [ForeignKey(nameof(TestTakerId))]
        public User TestTaker { get; set; }
        public float Score { get; set; }
        public DateTime TakenOn { get; set; }
    }
}
