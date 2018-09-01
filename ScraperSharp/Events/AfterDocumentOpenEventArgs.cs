namespace ScraperSharp.Events
{
    using System;
    using AngleSharp.Dom;

    public class AfterDocumentOpenEventArgs
        : EventArgs
    {
        public IDocument Document { get; set; }
    }
}