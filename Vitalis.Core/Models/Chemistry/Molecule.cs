using System.Text;

namespace Vitalis.Core.Models.Chemistry
{
    public class Molecule
    {
        public Molecule(StringBuilder mol)
        {
            Formula = mol;

        }
        public StringBuilder Formula { get; }
    }
}
