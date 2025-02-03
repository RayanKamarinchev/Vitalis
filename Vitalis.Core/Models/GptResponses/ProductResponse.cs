namespace Vitalis.Core.Models.GptResponses
{
    public class ProductResponse
    {
        public string Reactant1 { get; set; }
        public string Reactant2 { get; set; }
        public string Explanation { get; set; }
        public string ReactionEquation { get; set; }
        public string Product { get; set; }
        public string ProductInSMILES { get; set; }
    }
}
