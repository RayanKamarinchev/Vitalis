using Vitalis.Core.Models.Chemistry;

namespace Vitalis.Core.Contracts
{
    public interface IMoleculeService
    {
        List<Reaction> GetPossibleReactions(string reactant);
    }
}
