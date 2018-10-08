namespace ScraperSharp.Events
{
    using System;
    using ScraperSharp.Entities;
    using ScraperSharp.Settings;

    public class PropertyScrapedEventArgs
        : EventArgs
    {
        public PropertySettings Settings { get; set; }

        public Property ScrapedProperty { get; internal set; }
    }
}