using System.Globalization;
using System.Text;
using Vitalis.Core.Contracts;
using Vitalis.Core.Models.Chemistry;

namespace Vitalis.Core.Services
{
    public class MoleculeComputationService : IMoleculeComputationService
    {
        public List<Atom> ConvertFileToMolecule(string mol)
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


        public string ConvertMoleculeToFile(Molecule mol)
        {
            StringBuilder molFile = new StringBuilder();
            int atomsCount = mol.Atoms.Count;
            molFile.AppendLine("\r\n  MJ240402                      \r\n");
            int bondsCount = mol.Atoms.Sum(x => x.Bonds.Count) / 2;
            molFile.AppendLine($"{FitCount(atomsCount)}{FitCount(bondsCount)}  0  0  0  0  0  0  0  0999 V2000");

            int[,] bondMatrix = new int[atomsCount, atomsCount];
            for (int i = 0; i < atomsCount; i++)
            {
                foreach (var bond in mol.Atoms[i].Bonds)
                {
                    int bondedAtomIndex = mol.Atoms.IndexOf(bond.Atom);
                    int smallerIndex = i < bondedAtomIndex ? i : bondedAtomIndex;
                    int biggerIndex = i > bondedAtomIndex ? i : bondedAtomIndex;
                    bondMatrix[smallerIndex, biggerIndex] = (int)bond.Type;
                }

                molFile.AppendLine($"   {FitNumber(mol.Atoms[i].X)}   {FitNumber(mol.Atoms[i].Y)}    0.0000 {mol.Atoms[i].Element}   0  0  0  0  0  0  0  0  0  0  0  0");
            }

            for (int i = 0; i < atomsCount; i++)
            {
                for (int j = 0; j < atomsCount; j++)
                {
                    if (bondMatrix[i, j] != 0)
                    {
                        molFile.AppendLine($"  {i + 1}  {j + 1}  {bondMatrix[i, j]}  0  0  0  0 ");
                    }
                }
            }
            molFile.AppendLine("M  END");

            return molFile.ToString();
        }

        private string FitNumber(double num)
        {
            return (num >= 0 ? " " : "") + $"{num:f4}";
        }

        private string FitCount(int count, int space = 3)
        {
            int numberLength = (int)Math.Floor(Math.Log10(count) + 1);
            return new string(' ', space - numberLength) + count;
        }
    }
}
