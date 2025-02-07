namespace Vitalis.Core.Models.Chemistry
{
    public class Atom
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Element { get; set; }
        public List<Bond> Bonds { get; set; }

        public int Connections => Bonds.Count;

        public Atom(double x, double y, string element)
        {
            X = x;
            Y = y;
            Element = element;
            Bonds = new List<Bond>();
        }

        public void AddBond(Atom atom, BondType type)
        {
            Bonds.Add(new Bond(atom, type));
            atom.Bonds.Add(new Bond(this, type));
        }
    }
}
