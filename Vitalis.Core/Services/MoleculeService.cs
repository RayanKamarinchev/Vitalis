using System.Diagnostics;
using Vitalis.Core.Contracts;

namespace Vitalis.Core.Services
{
    public class MoleculeService : IMoleculeService
    {
        public string ConvertFile(string text, string inputFormat = "mrv", string outputFormat = "smi")
        {
            string obabelPath = "pyhon.exe";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = obabelPath,
                Arguments = $"-i{inputFormat} -o{outputFormat}",
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = startInfo })
            {
                process.Start();

                using (var writer = process.StandardInput)
                {
                    writer.WriteLine(text);
                }

                string smilesOutput = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();

                process.WaitForExit();

                //if (!string.IsNullOrEmpty(smilesOutput))
                //{
                    return smilesOutput.Substring(0, smilesOutput.IndexOf('\t'));
                //}
                throw new Exception();
            }
        }
    }
}
