using MathNet.Numerics;
using System.ComponentModel.DataAnnotations;

namespace Vitalis.Core.Models.Tests
{
    public class Filter
    {
        public string SearchTerm { get; set; }
        public Sorting Sorting { get; set; }
        public List<OrganicGroup> Groups { get; set; }
        public int Grade { get; set; }
    }
}
