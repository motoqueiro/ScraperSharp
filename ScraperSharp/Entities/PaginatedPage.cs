namespace ScraperSharp.Entities
{
    using System.Collections.Generic;

    public class PaginatedPage
    {
        public string Url { get; set; }

        public IEnumerable<Property> Properties { get; set; }

        public IDictionary<int, IEnumerable<Property>> ListProperties { get; set; }

        public Pagination Pagination { get; set; }
    }
}