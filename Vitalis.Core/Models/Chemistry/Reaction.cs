namespace Vitalis.Core.Models.Chemistry
{
    public class Reaction
    {
        public Reaction(string reagent, string catalyst, string conditions, string followUp)
        {
            Reagent = reagent;
            Catalyst = catalyst;
            Conditions = conditions;
            FollowUp = followUp;
        }

        public string? Reagent { get; set; }
        public string? Catalyst { get; set; }
        public string? Conditions { get; set; }
        public string? FollowUp { get; set; }
    }
}
