namespace ScraperSharp.Events
{
    using System.Collections.Generic;
    using ScraperSharp.Entities;

    public class ListPageScrapedEventArgs
        : BaseScrapedEventArgs
    {
        public IDictionary<int, IEnumerable<Property>> ListProperties { get; set; }
    }
}