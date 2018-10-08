namespace ScraperSharp.Entities
{
    using System.Collections.Generic;
    using AngleSharp.Dom;

    public class Pagination
    {
        public IElement TotalPages { get; set; }

        public IElement FirstPage { get; set; }

        public IElement PreviousPage { get; set; }

        public IList<IElement> Pages { get; set; }

        public IElement CurrentPage { get; set; }

        public IElement NextPage { get; set; }

        public IElement LastPage { get; set; }

        internal bool HasNextPage => this.NextPage != null;

        internal bool HasPage(int pageNumber) => this.Pages?[pageNumber - 1] != null;
    }
}