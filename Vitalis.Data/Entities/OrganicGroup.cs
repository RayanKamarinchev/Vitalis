using System.ComponentModel.DataAnnotations;

namespace Vitalis.Data.Entities
{
    public class OrganicGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<TestOrganicGroup> TestOrganicGroups { get; set; }
    }
}
