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

        //compution coordinates
        public static readonly double bondLength = 0.825;
    }
}
