using System.Collections.Generic;
using System.Globalization;
using Vitalis.Core.Models.Chemistry;
using Molecule = Vitalis.Core.Models.Chemistry.Molecule;

namespace Vitalis.Core.Domain
{
    public class ReactionPredictor
    {
        private readonly List<string> hydrogenReagents = ["HCl", "HBr", "NH3", "HCN"];
        private readonly List<string> bases = ["NaOH", "KOH"];

        public List<Molecule> PredictProduct(string reactantInMol, Reaction reaction, string reactantSmiles)
        {
            Molecule molecule = new Molecule(ConvertToMolecule(
                "\r\n  MJ240402                      \r\n\r\n  5  4  0  0  0  0  0  0  0  0999 V2000\r\n   -0.1450   -0.4687    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    0.5693   -0.0562    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    1.2838   -0.4687    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    1.9983   -0.0562    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n    2.7127   -0.4687    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0\r\n  1  2  1  0  0  0  0\r\n  2  3  1  0  0  0  0\r\n  3  4  1  0  0  0  0\r\n  4  5  1  0  0  0  0\r\nM  END"));
            molecule.Smiles = ChemConstants.smilesParser.ParseSmiles(reactantSmiles);
            List<Molecule> possibleProducts = new List<Molecule>();

            //halogens
            if (hydrogenReagents.Contains(reaction.Reagent) || bases.Contains(reaction.Reagent)
                                                            || reaction.Reagent == "H2O")
            {
                if (reaction.Conditions == "alcohol")
                {
                    return EliminationWithBondCreation(molecule, ["Cl", "Br"]);
                }

                possibleProducts.AddRange(Swap(molecule, ["Cl", "Br"]
                    , reaction.Reagent.Replace("H", "")));
            }

            //dehydration
            if (reaction.Reagent == "")
            {
                return EliminationWithBondCreation(molecule, ["O"]);
            }

            if (hydrogenReagents.Contains(reaction.Reagent))
            {
                possibleProducts.AddRange(Swap(molecule, ["O"]
                    , reaction.Reagent.Replace("H", "")));
            }

            if (ChemConstants.carbonylPattern.Matches(molecule.Smiles))
            {
                if (reaction.Reagent == "H2O")
                {
                    possibleProducts.AddRange(BondElimination(molecule));
                }

                if (reaction.Reagent == "HCN")
                {
                    var res = BondElimination(molecule)[0];
                    possibleProducts.AddRange(CarbonylAddition(res, "CN"));
                }
            }

            //eterification
            //if (reaction.Conditions== "t = 140C" && reaction.Catalyst == "H2SO4")
            //{
            //    possibleProducts.AddRange(Eterification());
            //}

            switch (reaction.Reagent)
            {
                case "Br2":
                case "Cl2":
                case "HNO3":
                    possibleProducts.AddRange(CommonReaction(molecule, reaction));
                    break;
                case "H2":
                case "HCl":
                case "HBr":
                case "H2O":
                    possibleProducts.AddRange(ComplexBondReaction(molecule, reaction));
                    break;
                case "NaNH2":
                    possibleProducts.AddRange(AlkyneNaReaction(molecule));
                    break;
                case "PCl3":
                case "PCl5":
                case "SOCl2":
                    possibleProducts.AddRange(Swap(molecule, ["O"], "Cl"));
                    break;
                case "PBr5":
                case "PBr3":
                    possibleProducts.AddRange(Swap(molecule, ["O"], "Br"));
                    break;
                case "LiAlH4":
                    if (ChemConstants.carboxylicAcidPattern.Matches(molecule.Smiles))
                    {
                        possibleProducts.AddRange(Elimination(molecule));
                    }
                    else if (ChemConstants.carbonylPattern.Matches(molecule.Smiles))
                    {
                        possibleProducts.AddRange(BondElimination(molecule));
                    }
                    break;
            }
        }

        private List<Molecule> CarbonylAddition(Molecule mol, string elementToAdd)
        {
            foreach (var atom in mol.Atoms)
            {
                if (atom.Element == "O" && atom.Bonds[0].Type == BondType.Double)
                {
                    mol.AddNewAtom(elementToAdd, atom.Bonds[0].Atom, BondType.Single);
                }
            }

            return [mol];
        }

        private List<Molecule> CommonReaction(Molecule mol, Reaction reaction)
        {
            if (ChemConstants.alkenesPattern.Matches(mol.Smiles) && ChemConstants.alkynesPattern.Matches(mol.Smiles)
                                                                 && reaction.Reagent != "HNO3")
            {
                ComplexBondReaction(mol, reaction);
            }

            Atom bondedAtom = mol.Atoms.Where(x => x.Connections != 4).MaxBy(x => x.Connections);
            string atomToAdd = reaction.Reagent switch
            {
                "Br2" => "Br",
                "Cl2" => "Cl",
                "HNO3" => "NO2"
            };
            mol.AddNewAtom(atomToAdd, bondedAtom, BondType.Single);
            return [mol];
        }

        private List<Molecule> ComplexBondReaction(Molecule mol, Reaction reaction)
        {
            foreach (var atom in mol.Atoms)
            {
                foreach (var bond in atom.Bonds)
                {
                    for (int i = 1; i < (int)bond.Type; i++)
                    {
                        if (reaction.Catalyst == "cat. Lindlar" && bond.Type != BondType.Triple)
                        {
                            break;
                        }

                        //by murkownikov`s law
                        Atom hydrogenTakingAtom = atom.Connections > bond.Atom.Connections ? bond.Atom : atom;
                        Atom otherAtom = hydrogenTakingAtom == atom ? bond.Atom : atom;
                        (string, string) atomsToAdd = reaction.Reagent switch
                        {
                            "Br2" => ("Br", "Br"),
                            "Cl2" => ("Cl", "Cl"),
                            "HCl" => ("H", "Cl"),
                            "HBr" => ("H", "Br"),
                            "HCN" => ("H", "CN"),
                            "H2O" => ("H", "O"),
                        };
                        mol.AddNewAtom(atomsToAdd.Item1, hydrogenTakingAtom, BondType.Single);
                        mol.AddNewAtom(atomsToAdd.Item2, otherAtom, BondType.Single);
                    }
                }
            }

            return [mol];
        }

        private List<Molecule> EliminationWithBondCreation(Molecule mol, string[] atomsToEliminate)
        {
            List<Molecule> possibleProducts = new List<Molecule>();
            foreach (var atom in mol.Atoms)
            {
                if (atomsToEliminate.Contains(atom.Element) && atom.Connections == 1)
                {
                    var bondedAtom = atom.Bonds[0].Atom;
                    var otherAtomPossibilities = bondedAtom.Bonds
                                                           .Where(x => x.Atom.Connections != 4)
                                                           .Select(x => mol.Atoms.IndexOf(x.Atom));

                    mol.Atoms.Remove(atom);
                    bondedAtom.Bonds.RemoveAll(x => x.Atom.Equals(atom));
                    int bondedAtomIndex = mol.Atoms.IndexOf(bondedAtom);

                    foreach (var otherAtomIndex in otherAtomPossibilities)
                    {
                        var newMol = new Molecule(mol);
                        mol.ChangeBond(newMol.Atoms[bondedAtomIndex], newMol.Atoms[otherAtomIndex], true);
                        possibleProducts.Add(newMol);
                    }
                }
            }

            return possibleProducts;
        }

        private List<Molecule> Elimination(Molecule mol)
        {
            foreach (var atom in mol.Atoms)
            {
                if (atom.Element == "O" && atom.Bonds[0].Type == BondType.Double)
                {
                    mol.Atoms.Remove(atom);
                    atom.Bonds[0].Atom.Bonds.RemoveAll(x => x.Atom.Equals(atom));
                }
            }

            return [mol];
        }

        private List<Molecule> BondElimination(Molecule mol)
        {
            foreach (var atom in mol.Atoms)
            {
                if (atom.Element == "O" && atom.Bonds[0].Type == BondType.Double)
                {
                    atom.Bonds[0].Type = BondType.Single;
                }
            }

            return [mol];
        }

        private List<Molecule> Swap(Molecule mol, string[] elementsToSwap, string newElement)
        {
            foreach (var atom in mol.Atoms)
            {
                if (elementsToSwap.Contains(atom.Element) && atom.Connections == 1)
                {
                    atom.Element = newElement;
                }
            }

            return [mol];
        }

        private List<Molecule> AlkyneNaReaction(Molecule mol)
        {
            var possibleAtoms = mol.Atoms.Where(x => x.Connections == 3 && x.Bonds.Count == 1);
            foreach (var atom in possibleAtoms)
            {
                mol.AddNewAtom("Na", atom, BondType.Single);
            }

            return [mol];
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
