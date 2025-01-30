using System.ComponentModel.DataAnnotations.Schema;

namespace Vitalis.Data.Entities
{
    public class TestOrganicGroup
    {
        [ForeignKey(nameof(TestId))]
        public Test Test { get; set; }
        public Guid TestId { get; set; }
        [ForeignKey(nameof(OrganicGroupId))]
        public OrganicGroup OrganicGroup { get; set; }
        public int OrganicGroupId { get; set; }
    }
}
