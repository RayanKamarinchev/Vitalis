using NCDK.FaulonSignatures.Chemistry;
using System.Globalization;
using Vitalis.Core.Models.Chemistry;
using Molecule = Vitalis.Core.Models.Chemistry.Molecule;

namespace Vitalis.Core.Domain
{
    public class ReactionPredictor
    {
        public string PredictProduct(string reactantInMol, Reaction reaction, string reactantSmiles)
        {
            Molecule molecule = new Molecule(ConvertToMolecule("\r\n  MJ240402                      \r\n\r\n  5  4  0  0  0  0  0  0  0  0999 V2000\r\n   -0.1450   -0.4687    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    0.5693   -0.0562    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    1.2838   -0.4687    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    1.9983   -0.0562    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    2.7127   -0.4687    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n  1  2  1  0  0  0  0\r\n  2  3  1  0  0  0  0\r\n  3  4  1  0  0  0  0\r\n  4  5  1  0  0  0  0\r\nM  END"));
            molecule.Smiles = ChemConstants.smilesParser.ParseSmiles(reactantSmiles);
            switch (reaction.Reagent)
            {
                case "Br2":
                case "Cl2":
                case "HNO3":
                    CommonReaction(molecule, reaction.Reagent);
                    break;
                case //TODO
            }
        }

        private void CommonReaction(Molecule mol, string reactant)
        {
            if (ChemConstants.alkenesPattern.Matches(mol.Smiles) && ChemConstants.alkynesPattern.Matches(mol.Smiles)
                && reactant != "HNO3")
            {
                ComplexBondReaction();
            }
            Atom bondedAtom = mol.Atoms.Where(x => x.Connections != 4).MaxBy(x => x.Connections);
            string atomToAdd = reactant switch
            {
                "Br2" => "Br",
                "Cl2" => "Cl",
                "HNO3" => "NO2"
            };
            mol.AddNewAtom(atomToAdd, bondedAtom, BondType.Single);
        }

        private void ComplexBondReaction(Molecule mol, string reactant)
        {
            foreach (var atom in mol.Atoms)
            {
                foreach (var bond in atom.Bonds)
                {
                    if (bond.Type != BondType.Single)
                    {
                        //by murkownikov`s law
                        Atom hydrogenTakingAtom = atom.Connections > bond.Atom.Connections ? bond.Atom : atom;
                        Atom otherAtom = hydrogenTakingAtom == atom ? bond.Atom : atom;
                        (string, string) atomsToAdd = reactant switch
                        {
                            "Br2" => ("Br", "Br"),
                            "Cl2" => ("Cl", "Cl"),
                            "HCl" => ("H", "Cl"),
                            "HBr" => ("H", "Br"),
                            "HCN" => ("H", "CN"),
                            "H2O" => ("H", "OH"),
                        };
                    }
                }
            }
        }

        private static List<Atom> ConvertToMolecule(string mol)
        {
            List<Atom> molecule = new List<Atom>();
            int startIndex = 3;
            var block = mol.Split(new char[] { '\n' });
            string str1 = block[startIndex];
            int atomsCount = int.Parse(str1.Substring(0, 3).Trim(), (IFormatProvider)NumberFormatInfo.InvariantInfo);
            int bondsCount = int.Parse(str1.Substring(3, 3).Trim(), (IFormatProvider)NumberFormatInfo.InvariantInfo);

            startIndex++;
            int lastAtomIndex = atomsCount + startIndex;
            for (int i = startIndex; i < lastAtomIndex; ++i)
            {
                var input = block[i].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
                molecule.Add(new Atom(double.Parse(input[0]), double.Parse(input[1]), input[3]));
            }
            if (atomsCount > 1)
            {
                int lastBondIndex = lastAtomIndex + bondsCount;
                for (int index = lastAtomIndex; index < lastBondIndex; ++index)
                {
                    string str2 = block[index];
                    var input = str2.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
                    int firstElement = int.Parse(input[0]);
                    int secondElement = int.Parse(input[1]);
                    BondType bondType = (BondType)int.Parse(input[2]);
                    molecule[firstElement - 1].Bonds.Add(new Bond(molecule[secondElement - 1], bondType));
                    molecule[secondElement - 1].Bonds.Add(new Bond(molecule[firstElement - 1], bondType));
                }
            }
            return molecule;
        }
    }
}
