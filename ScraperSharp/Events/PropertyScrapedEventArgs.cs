namespace ScraperSharp.Events
{
    using System;
    using ScraperSharp.Entities;

    public class PropertyScrapedEventArgs
        : EventArgs
    {
        public Property ScrapedProperty { get; internal set; }
    }
}