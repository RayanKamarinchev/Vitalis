using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI.Chat;
using Vitalis.Core.Contracts;
using Vitalis.Core.Models.Chemistry;
using static Vitalis.Core.Constants.Constants;
using System.Text.RegularExpressions;

namespace Vitalis.Core.Services
{
    public class MoleculeService : IMoleculeService
    {
        private readonly IConfiguration config;

        public MoleculeService(IConfiguration _config)
        {
            config = _config;
        }

        public List<Reaction> GetPossibleReactions(string reactant)
        {
            //cycloalkanes 2 halogens
            //dehalogenation
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

            //geminal halogens
            if (Regex.IsMatch(reactant, "\\((Cl|Br)\\)\\((Cl|Br)\\)"))
            {
                AddReaction("NaNH2", "strong base", "t");
            }

            if (IsSoldiumAlkynide(reactant))
            {
                
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

            if (mol.Count(x=>x=='C') < 3)
            {
                return false;
            }

            int endOfBoundLen = endOfBond.Length;

            if (mol[len-2 - endOfBoundLen] == '#' && mol[len-1 - endOfBoundLen] == 'C')
            {
                return true;
            }

            if (mol[1] == '#' && mol[0] == 'C')
            {
                return true;
            }

            return false;
        }

        private bool MatchPattern(string mol, string match)
        {
            //match: RC#C
            GetBranches(mol);
        }

        private List<string> GetBranches(string mol)
        {
            List<string> branches = new List<string>();

        }

        static string GetTextBetween(string text, string start, string end)
        {
            int startIndex = text.IndexOf(start) + 1;
            int endIndex = text.IndexOf(end, startIndex);
            string result = text.Substring(startIndex, endIndex - startIndex);
            return result;
        }

        private bool ContainsHexagon(string mol)
        {
            string newMol = Regex.Replace(mol, "\\(\\w+\\)", "");
            return Regex.IsMatch(newMol, "(?<g>[0-9]+)C{5}\\k<g>");
        }
    }
}
