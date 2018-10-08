namespace ScraperSharp.Events
{
    using System.Collections.Generic;
    using ScraperSharp.Entities;
    using ScraperSharp.Settings;

    public class PageScrapedEventArgs
        : BaseScrapedEventArgs
    {
        public PageSettings Settings { get; set; }

        public IEnumerable<Property> Properties { get; internal set; }
    }
}