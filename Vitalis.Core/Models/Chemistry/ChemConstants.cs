using NCDK.SMARTS;
using NCDK.Smiles;

namespace Vitalis.Core.Models.Chemistry
{
    public static class ChemConstants
    {
        public static readonly SmilesParser smilesParser = new SmilesParser();
        public static readonly SmartsPattern benzenePattern = SmartsPattern.Create("c1ccccc1");
        public static readonly SmartsPattern acidPropertiesAlkynePattern = SmartsPattern.Create("[C;H1]#[C;H0]");
        public static readonly SmartsPattern soldiumAlkynePattern = SmartsPattern.Create("[C;Na1]#[C;H0]");
        public static readonly SmartsPattern geminalHalogensPattern = SmartsPattern.Create("[CX4]([F,Cl,Br,I])[F,Cl,Br,I]");
        public static readonly SmartsPattern alcoholsPattern = SmartsPattern.Create("[CX4][OH]");
        public static readonly SmartsPattern alkenesPattern = SmartsPattern.Create("[C]=[C]");
        public static readonly SmartsPattern alkynesPattern = SmartsPattern.Create("[C]#[C]");
        public static readonly SmartsPattern halogensPattern = SmartsPattern.Create("[C][Cl,Br]");
        public static readonly SmartsPattern carbonylPattern = SmartsPattern.Create("[C]=[O]");
        public static readonly SmartsPattern aminesPattern = SmartsPattern.Create("[NX3;!$(N=*)]");
        public static readonly SmartsPattern soldiumAlkoxidePattern = SmartsPattern.Create("[Na+].[O-][C]");
        public static readonly SmartsPattern nitrilesPattern = SmartsPattern.Create("[C]#N");
        public static readonly SmartsPattern amidesPattern = SmartsPattern.Create("[CX3](=[O])[NX3]");
        public static readonly SmartsPattern nitroarenesPattern = SmartsPattern.Create("[a][N+](=O)[O-]");
        public static readonly SmartsPattern aminoarenesPattern = SmartsPattern.Create("[a][NH2]");
        public static readonly SmartsPattern diazoniumPattern = SmartsPattern.Create("[a][N+]#N");
        public static readonly SmartsPattern carboxylicAcidPattern = SmartsPattern.Create("[CX3](=O)[OX2H]");
        public static readonly SmartsPattern hexagonPattern = SmartsPattern.Create("C1CCCCC1");

        public static readonly List<string> hydrogenReagents = ["HCl", "HBr", "NH3", "HCN"];
        public static readonly List<string> bases = ["NaOH", "KOH"];

        //compution coordinates
        public static readonly double bondLength = 0.825;

        public static readonly List<ReactionPattern> reactionPatterns = new List<ReactionPattern>()
        {
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.hexagonPattern,
                Reactions = new List<Reaction> { new("", "", "t, cat") }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.halogensPattern,
                Reactions = new List<Reaction>
                {
                    new("KOH", "", "alcohol"),
                    new("KOH", "", "aqua"),
                    new("H2O", "", ""),
                    new("NH3", "", ""),
                    new("NaCN", "", ""),
                    //TODO
                    new("Li", "", ""),
                    new("Mg", "", "")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.alkenesPattern,
                Reactions = new List<Reaction>
                {
                    //TODO
                    //dehydrogenation
                    new("", "", "t"),

                    new("HCl", "", ""),
                    new("HBr", "", ""),
                    new("H2", "Ni", "t, p"),
                    new("HCN", "", ""),
                    new("H2O", "H+", "t, p"),
                    new("H2O + KMnO4", "", ""),//TODO
                    new("KMnO4", "", "OH-, t", "H2SO4"),//TODO
                    new("O2", "", "300 C", "Ag"),//TODO
                    new("O2", "PdCl2 . CuCl2", "t")//TODO
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.alkynesPattern,
                Reactions = new List<Reaction>
                {
                    new("H2", "cat. Lindlar", "aqua")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = acidPropertiesAlkynePattern,
                Reactions = new List<Reaction>
                {
                    new("NaNH2", "", "")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.alcoholsPattern,
                Reactions = new List<Reaction>
                {
                    new("", "H2SO4", "t = 180C"),
                    new("", "Pt", "t"),
                    new("Na", "", "t"),//TODO
                    new("HCl", "", ""),
                    new("HBr", "", ""),
                    new("NH3", "Al2O3", "t"),
                    new("HCN", "", ""),
                    new("PBr3", "", "t < 0C"),
                    new("PCl3", "", "t < 0C"),
                    new("PCl5", "", "t < 0C"),
                    new("PBr5", "", "t < 0C"),
                    new("SOCl2", "", ""),
                    new("CH3CH2OH", "H2SO4", "t = 140C"),//TODO
                    new("CH3COOH", "H+", "t"),//TODO
                    new("KMnO4", "K2Cr2O7", ""),//TODO
                    new("NaOH + I2", "", "")//TODO
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.carbonylPattern,
                Reactions = new List<Reaction>
                {
                    new("LiAlH4", "", ""),
                    new("H2O", "", ""),
                    new("HCN", "", ""),
                    new("NaHSO3", "", ""),
                    new("KMnO4", "", ""),
                    new("", "NaBH4", "", "H+")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.carboxylicAcidPattern,
                Reactions = new List<Reaction>
                {
                    new("Na", "", "t"),
                    new("NaOH", "", "t"),
                    new("NH3", "", "t"),
                    new("CH3CH2OH", "", "t"),
                    new("PCl3", "", "t"),
                    new("PCl5", "", "t"),
                    new("SOCl2", "", "t"),
                    new("LiAlH4", "", ""),
                    new("", "", "t")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.benzenePattern,
                Reactions = new List<Reaction>
                {
                    new("CH3COCl", "AlCl3 (lewis acid)", ""),
                    new("H2", "", "t, p"),
                    new("KMnO4", "", "OH-, t", "H2SO4"),
                    new("HCl", "", ""),
                    new("HBr", "", ""),
                    new("HCN", "", "")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.diazoniumPattern,
                Reactions = new List<Reaction>
                {
                    new("CuCN", "", "t"),
                    new("H3PO2 + H2O", "", "t"),
                    new("H2O", "", "t"),
                    new("CuBr", "", "t")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.nitrilesPattern,
                Reactions = new List<Reaction>
                {
                    new("LiAlH4", "", ""),
                    new("H2O", "", "")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.amidesPattern,
                Reactions = new List<Reaction> { new("LiAlH4", "", "") }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.nitroarenesPattern,
                Reactions = new List<Reaction> { new("H2", "Ni", "t, p") }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.aminoarenesPattern,
                Reactions = new List<Reaction> { new("NaNO2 + HCl", "", "t = 0C") }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.geminalHalogensPattern,
                Reactions = new List<Reaction>
                {
                    new("NaNH2", "strong base", "t"),
                    new("H2O", "", "")
                }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.soldiumAlkynePattern,
                Reactions = new List<Reaction> { new("ClCH2R", "", "") }
            },
            new ReactionPattern()
            {
                SmartsPattern = ChemConstants.soldiumAlkoxidePattern,
                Reactions = new List<Reaction>
                {
                    new("ClCH2CH3", "", ""),
                    new("BrCH2CH3", "", "")
                }
            }
        };
    }
}
