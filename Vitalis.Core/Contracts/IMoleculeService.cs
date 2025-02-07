using Vitalis.Core.Models.Chemistry;
using Vitalis.Core.Models.GptResponses;

namespace Vitalis.Core.Contracts
{
    public interface IMoleculeService
    {
        List<Reaction> GetPossibleReactions(string reactant);
        Task<string> PredictProduct(string reactant, Reaction reaction);
        Task<CompoundInfo> GetInfo(string name);
    }
}
