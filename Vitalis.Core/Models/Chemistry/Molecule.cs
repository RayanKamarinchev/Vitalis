using MathNet.Numerics;
using NCDK;

namespace Vitalis.Core.Models.Chemistry
{
    public class Molecule
    {
        public List<Atom> Atoms { get; set; }
        public IAtomContainer Smiles { get; set; }
        public Molecule(List<Atom> atoms)
        {
            Atoms = atoms;
        }

        public Molecule(Molecule mol)
        {
            Atoms = new List<Atom>(mol.Atoms);
            Smiles = mol.Smiles;
        }

        public void AddNewAtom(string atomElement, Atom bondedAtom, BondType type)
        {
            var (x, y) = GetCoords(bondedAtom);
            Atom newAtom = new Atom(x, y, atomElement);
            Atoms.Add(newAtom);
            bondedAtom.AddBond(newAtom, type);
        }
        public void ChangeBond(Atom atom1, Atom atom2, bool addBond)
        {
            atom1.Bonds.FirstOrDefault(x => x.Atom.Equals(atom2)).Type += addBond ? 1 : -1;
            atom2.Bonds.FirstOrDefault(x => x.Atom.Equals(atom1)).Type += addBond ? 1 : -1;
        }

        private (double, double) GetCoords(Atom bondedAtom)
        {
            double x = bondedAtom.X;
            double y = bondedAtom.Y;
            Atom[] connectedAtoms = bondedAtom.Bonds.Select(x => x.Atom).ToArray();
            Atom orientationalAtom = connectedAtoms[0];
            double dx = orientationalAtom.X - x;
            double dy = orientationalAtom.Y - y;
            double newX, newY;
            if (dx != 0)
            {
                newX = x - dx;
                newY = y - dy;
                if (NoAtomsAtCoords(newX, newY, connectedAtoms))
                {
                    return (newX, y + newY);
                }

                newX = x;
                newY = y - ChemConstants.bondLength * (dy / Math.Abs(dy));
                //if the slope of the last connected atom points down we need to check the reverse direction
                if (NoAtomsAtCoords(newX, newY, connectedAtoms))
                {
                    return (newX, newY);
                }

                return (x, y + ChemConstants.bondLength * (dy / Math.Abs(dy)));
            }
            else
            {
                newX = x + ChemConstants.bondLength * Math.Cos(30);
                newY = y - ChemConstants.bondLength * Math.Sin(30) * (dx / Math.Abs(dx));
                if (NoAtomsAtCoords(newX, newY, connectedAtoms))
                {
                    return (newX, newY);
                }
                newX = x - ChemConstants.bondLength * Math.Cos(30);
                if (NoAtomsAtCoords(newX, newY, connectedAtoms))
                {
                    return (newX, newY);
                }

                return (x, y - ChemConstants.bondLength);
            }
        }

        private bool NoAtomsAtCoords(double x, double y, Atom[] atomsToCheck)
        {
            foreach (var atom in atomsToCheck)
            {
                if (atom.X.AlmostEqual(x, 2) && atom.Y.AlmostEqual(y, 2))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
