using Vitalis.Data.Entities;

namespace Vitalis.Core.Models.Tests
{
    public class QueryModel<T>
    {
        public QueryModel()
        {
            Filters = new Filter();
        }

        public QueryModel(string SearchTerm, int Grade, List<int> groups, Sorting Sorting, int currentPage)
        {
            Filters = new Filter();
            Filters.SearchTerm = SearchTerm;
            Filters.Grade = Grade;
            Filters.Groups = groups;
            Filters.Sorting = Sorting;
            CurrentPage = currentPage;
        }
        public int ItemsPerPage { get; set; } = 6;
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public Filter Filters { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}
