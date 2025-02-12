using Vitalis.Core.Models.Chemistry;

namespace Vitalis.Core.Contracts
{
    public interface IProductPredictionService
    {
        List<Reaction> GetPossibleReactions(string reactant);
        string PredictProduct(string reactant, string smiles, Reaction reaction);
    }
}
