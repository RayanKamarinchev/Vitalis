using Microsoft.Extensions.Configuration;
using NCDK.SMARTS;
using NCDK.Smiles;
using NCDK;
using System.Text.RegularExpressions;
using Vitalis.Core.Contracts;
using Vitalis.Core.Models.Chemistry;

namespace Vitalis.Core.Services
{
    public class MoleculeService : IMoleculeService
    {
        private readonly IConfiguration config;
        private readonly SmilesParser smilesParser = new SmilesParser();
        private readonly SmartsPattern benzenePattern = SmartsPattern.Create("c1ccccc1");

        public MoleculeService(IConfiguration _config)
        {
            config = _config;
        }

        public List<Reaction> GetPossibleReactions(string reactant)
        {
            //cycloalkanes 2 halogens
            //dehalogenation
            //benxene preparation
            var mol = smilesParser.ParseSmiles(reactant);

            List<Reaction> reactions = new List<Reaction>();
            AddReaction("Cl2", "", "hv");
            AddReaction("Br2", "", "hv");
            AddReaction("HNO3", "", "t");
            AddReaction("H2SO4", "", "t");

            //dehydration
            AddReaction("", "", "t");

            if (ContainsHexagon(reactant))
            {
                AddReaction("", "", "t, cat");
            }


            if (reactant.Contains("Cl") || reactant.Contains("Br"))
            {
                AddReaction("KOH", "", "alcohol");
                AddReaction("KOH", "", "aqua");
            }

            //alkene alkine
            if (reactant.Contains('=') || reactant.Contains('#'))
            {
                AddReaction("HCl", "", "");
                AddReaction("HBr", "", "");
                AddReaction("H2", "Ni", "t, p");
                AddReaction("HCN", "", "");
                AddReaction("H2O + KMnO4", "", "");
                AddReaction("KMnO4", "", "OH-, t", "H2SO4");
                AddReaction("O2", "", "300 C", "Ag");
            }

            if (reactant.Contains('#'))
            {
                AddReaction("", "cat. Lindlar", "aqua");
                if (IsAlkyneWithTripleBondAtTheEnd(reactant))
                {
                    AddReaction("NaNH2", "", "");
                }
            }

            //alchohol
            if (reactant.Contains("(O)"))
            {
                //dehydration
                AddReaction("", "H2SO4", "t");
            }

            //has benzene nucleus
            if (benzenePattern.Matches(mol))
            {
                
            }

            //geminal halogens
            if (Regex.IsMatch(reactant, "\\((Cl|Br)\\)\\((Cl|Br)\\)"))
            {
                AddReaction("NaNH2", "strong base", "t");
            }

            if (IsAlkyneWithTripleBondAtTheEnd(reactant, "Na"))
            {
                AddReaction("ClCH2R", "", "");
            }

            void AddReaction(string reagent, string catalyst, string conditions, string followUp = "")
            {
                reactions.Add(new Reaction(reagent, catalyst, conditions, followUp));
            }
        }

        private bool IsAlkyneWithTripleBondAtTheEnd(string mol, string endOfBond = "")
        {
            int len = mol.Length;
            if (len < 2)
            {
                return false;
            }

            if (mol.Count(x => x == 'C') < 3)
            {
                return false;
            }

            int endOfBoundLen = endOfBond.Length;

            if (mol[len - 2 - endOfBoundLen] == '#' && mol[len - 1 - endOfBoundLen] == 'C')
            {
                return true;
            }

            if (mol[1 + endOfBoundLen] == '#' && mol[endOfBoundLen] == 'C' && mol.StartsWith(endOfBond))
            {
                return true;
            }

            return false;
        }


        private bool ContainsHexagon(string mol)
        {
            //TODO
            string newMol = Regex.Replace(mol, "\\(\\w+\\)", "");
            return Regex.IsMatch(newMol, "(?<g>[0-9]+)C{5}\\k<g>");
        }
    }
}
