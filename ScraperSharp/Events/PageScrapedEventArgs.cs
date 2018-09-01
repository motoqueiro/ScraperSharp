namespace ScraperSharp.Events
{
    using System.Collections.Generic;
    using ScraperSharp.Entities;

    public class PageScrapedEventArgs
        : BaseScrapedEventArgs
    {
        public IEnumerable<Property> Properties { get; internal set; }
    }
}