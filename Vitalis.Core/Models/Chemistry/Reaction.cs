using System.Text;

namespace Vitalis.Core.Models.Chemistry
{
    public class Reaction
    {
        public string? Reagent { get; set; }
        public string? ReagentVisualized { get; set; }
        public string? Catalyst { get; set; }
        public string? Conditions { get; set; }
        public string? FollowUp { get; set; }

        public Reaction(string reagent, string catalyst, string conditions, string followUp = "")
        {
            Reagent = reagent;
            Catalyst = catalyst;
            Conditions = conditions;
            FollowUp = followUp;

            StringBuilder reagentVisualized = new StringBuilder(reagent);
            for (int i = 0; i < reagent.Length; i++)
            {
                if (char.IsDigit(reagentVisualized[i]))
                {
                    reagentVisualized[i] = subDigits[reagent[i]];
                }
            }

            ReagentVisualized = reagentVisualized.ToString();
        }

        private readonly Dictionary<char, char> subDigits = new Dictionary<char, char>()
        {
            { '2', '\u2082' },
            { '3', '\u2083' },
            { '4', '\u2084' },
            { '5', '\u2085' },
            { '6', '\u2086' },
            { '7', '\u2087' },
            { '8', '\u2088' },
            { '9', '\u2089' },
        };
    }
}
