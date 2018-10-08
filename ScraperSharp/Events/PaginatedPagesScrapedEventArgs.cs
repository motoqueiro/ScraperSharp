namespace ScraperSharp.Events
{
    using System.Collections.Generic;
    using ScraperSharp.Entities;
    using ScraperSharp.Settings;

    public class PaginatedPagesScrapedEventArgs
    {
        public PaginatedPagesSettings Settings { get; set; }

        public IEnumerable<PaginatedPage> Pages { get; set; }
    }
}