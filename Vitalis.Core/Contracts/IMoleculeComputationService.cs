using Vitalis.Core.Models.Chemistry;

namespace Vitalis.Core.Contracts
{
    public interface IMoleculeComputationService
    {
        List<Atom> ConvertFileToMolecule(string mol);
        string ConvertMoleculeToFile(Molecule mol);
    }
}
