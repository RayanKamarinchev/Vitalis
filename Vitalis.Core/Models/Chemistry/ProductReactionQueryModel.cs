namespace Vitalis.Core.Models.Chemistry
{
    public class ProductReactionQueryModel
    {
        public string Reactant { get; set; }
        public string Smiles { get; set; }
        public string Reagent { get; set; }
        public string Catalyst { get; set; } = "";
        public string Conditions { get; set; } = "";
        public string FollowUp { get; set; } = "";
    }
}
