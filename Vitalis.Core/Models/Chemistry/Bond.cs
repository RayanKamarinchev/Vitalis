namespace Vitalis.Core.Models.Chemistry
{
    public class Bond
    {
        public Atom Atom { get; set; }
        public BondType Type { get; set; }

        public Bond(Atom atom, BondType type)
        {
            Atom = atom;
            Type = type;
        }
    }
}
