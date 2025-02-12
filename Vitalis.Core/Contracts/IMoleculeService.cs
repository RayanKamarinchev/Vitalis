using Vitalis.Core.Models.Chemistry;
using Vitalis.Core.Models.GptResponses;

namespace Vitalis.Core.Contracts
{
    public interface IMoleculeService
    {
        Task<CompoundInfo> GetInfo(string name);
    }
}
