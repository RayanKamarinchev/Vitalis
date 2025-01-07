namespace Vitalis.Core.Models.Tests
{
    public class TestResultsViewModel
    {
        public Guid TestId { get; set; }
        public DateTime TakenOn { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public float Score { get; set; }
    }
}
