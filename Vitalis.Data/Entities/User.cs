using Microsoft.AspNetCore.Identity;

namespace Vitalis.Data.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<TestLike> TestLikes { get; set; }
        public ICollection<Test> Tests { get; set; }
    }
}
