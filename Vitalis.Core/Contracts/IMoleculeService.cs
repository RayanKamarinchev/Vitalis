namespace Vitalis.Core.Contracts
{
    public interface IMoleculeService
    {
        string ConvertFile(string text, string inputFormat = "mrv", string outputFormat = "smi");
    }
}
