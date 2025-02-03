using System.ComponentModel.DataAnnotations;

namespace Vitalis.Data.Entities
{
    public class Compound
    {
        [Key] 
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhysicalAppearance { get; set; }
        public string Applications { get; set; }
    }
}
