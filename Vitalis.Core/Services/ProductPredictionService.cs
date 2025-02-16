using Vitalis.Core.Contracts;
using Vitalis.Core.Models.Chemistry;

namespace Vitalis.Core.Services
{
    public class ProductPredictionService : IProductPredictionService
    {
        private readonly IMoleculeComputationService moleculeComputationService;

        public ProductPredictionService(IMoleculeComputationService _moleculeComputationService)
        {
            moleculeComputationService = _moleculeComputationService;
        }

        public List<Reaction> GetPossibleReactions(string reactant)
        {
            var mol = ChemConstants.smilesParser.ParseSmiles(reactant);

            List<Reaction> reactions = new List<Reaction>();
            AddReaction("Cl2", "", "hv");
            AddReaction("Br2", "", "hv");
            AddReaction("HNO3", "", "t");

            foreach (var reactionPattern in ChemConstants.reactionPatterns)
            {
                if (reactionPattern.SmartsPattern.Matches(mol))
                {
                    reactions.AddRange(reactionPattern.Reactions);
                }
            }

            return reactions;

            void AddReaction(string reagent, string catalyst, string conditions, string followUp = "")
            {
                reactions.Add(new Reaction(reagent, catalyst, conditions, followUp));
            }
        }

        public string PredictProduct(string reactant, string smiles, Reaction reaction)
        {
            var predictions = PredictProducts(reactant, reaction, smiles);
            return moleculeComputationService.ConvertMoleculeToFile(predictions[0]);
        }

        private List<Molecule> PredictProducts(string reactantInMol, Reaction reaction, string reactantSmiles)
        {
            Molecule molecule = new Molecule(moleculeComputationService.ConvertFileToMolecule(reactantInMol));
            molecule.Smiles = ChemConstants.smilesParser.ParseSmiles(reactantSmiles);
            List<Molecule> possibleProducts = new List<Molecule>();

            //halogens
            if (ChemConstants.hydrogenReagents.Contains(reaction.Reagent) || ChemConstants.bases.Contains(reaction.Reagent)
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

            if (ChemConstants.hydrogenReagents.Contains(reaction.Reagent))
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

            return possibleProducts;
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
            if ((ChemConstants.alkenesPattern.Matches(mol.Smiles) || ChemConstants.alkynesPattern.Matches(mol.Smiles))
                                                                 && reaction.Reagent != "HNO3")
            {
                ComplexBondReaction(mol, reaction);
                return [mol];
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
            for (int i = 0; i < mol.Atoms.Count; i++)
            {
                var atom = mol.Atoms[i];
                for (int j = 0; j < atom.Bonds.Count; j++)
                {
                    var bond = atom.Bonds[j];
                    if ((int)bond.Type > 1)
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
                            "H2" => ("H", "H")
                        };
                        if (atomsToAdd.Item1 != "H")
                        {
                            mol.AddNewAtom(atomsToAdd.Item1, hydrogenTakingAtom, BondType.Single);
                        }
                        if (atomsToAdd.Item2 != "H")
                        {
                            mol.AddNewAtom(atomsToAdd.Item2, otherAtom, BondType.Single);
                        }

                        bond.Type--;
                        bond.Atom.Bonds.FirstOrDefault(x => x.Atom.Equals(atom)).Type--;
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
    }
}
