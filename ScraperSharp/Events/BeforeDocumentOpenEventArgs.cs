using AngleSharp;

namespace ScraperSharp.Events
{
    public class BeforeDocumentOpenEventArgs
    {
        public IBrowsingContext BrowsingContext { get; set; }

        public string Url { get; set; }
    }
}