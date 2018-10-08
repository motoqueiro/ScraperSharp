namespace ScraperSharp.Events
{
    using System.Collections.Generic;
    using ScraperSharp.Entities;
    using ScraperSharp.Settings;

    public class ListPageScrapedEventArgs
        : BaseScrapedEventArgs
    {
        public ListPageSettings Settings { get; set; }

        public IEnumerable<Property> Properties { get; set; }

        public IDictionary<int, IEnumerable<Property>> ListProperties { get; set; }
    }
}