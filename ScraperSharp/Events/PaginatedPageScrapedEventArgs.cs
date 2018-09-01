namespace ScraperSharp.Events
{
    using ScraperSharp.Entities;

    public class PaginatedPageScrapedEventArgs
        : BaseScrapedEventArgs
    {
        public PaginatedPage PaginatedPage { get; internal set; }
    }
}