namespace ScraperSharp.Entities
{
    using System.Collections.Generic;

    public class PaginatedPage
    {
        public IDictionary<int, IEnumerable<Property>> ListProperties { get; set; }

        public Pagination Pagination { get; set; }
    }
}