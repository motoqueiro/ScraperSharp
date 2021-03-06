﻿namespace ScraperSharp.Events
{
    using ScraperSharp.Entities;
    using ScraperSharp.Settings;

    public class PaginatedPageScrapedEventArgs
        : BaseScrapedEventArgs
    {
        public PaginatedPagesSettings Settings { get; set; }

        public PaginatedPage PaginatedPage { get; internal set; }
    }
}