using NCDK.SMARTS;

namespace Vitalis.Core.Models.Chemistry
{
    public class ReactionPattern
    {
        public SmartsPattern SmartsPattern { get; set; }
        public List<Reaction> Reactions { get; set; }
    }
}
