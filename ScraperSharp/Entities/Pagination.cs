namespace ScraperSharp.Entities
{
    using System.Collections.Generic;
    using AngleSharp.Dom;
    using AngleSharp.Dom.Html;

    public class Pagination
    {
        public int? TotalPages { get; set; }

        public IHtmlAnchorElement FirstPage { get; set; }

        public IHtmlAnchorElement PreviousPage { get; set; }

        public IEnumerable<IHtmlAnchorElement> Pages { get; set; }

        public IElement CurrentPage { get; set; }

        public IHtmlAnchorElement NextPage { get; set; }

        public IHtmlAnchorElement LastPage { get; set; }
    }
}