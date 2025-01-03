using Vitalis.Core.Models.Chemistry;

namespace Vitalis.Core.Contracts
{
    public interface IMoleculeService
    {
        List<Reaction> GetPossibleReactions(string reactant);
        Task<string> PredictProduct(string reactant, string reagent, string catalyst, string conditions,
            string followUp);
    }
}
