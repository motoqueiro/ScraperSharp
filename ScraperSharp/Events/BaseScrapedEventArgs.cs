namespace ScraperSharp.Events
{
    using System;

    public class BaseScrapedEventArgs
        : EventArgs
    {
        public string Url { get; internal set; }
    }
}