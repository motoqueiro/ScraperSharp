namespace ScraperSharp.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AngleSharp.Dom;
    using AngleSharp.Dom.Html;
    using Microsoft.Extensions.Logging;
    using ScraperSharp.Entities;
    using ScraperSharp.Events;
    using ScraperSharp.Settings;

    public class PaginatedPagesScraper
        : ListPageScraper
    {
        public PaginatedPagesScraper(
            PaginatedPagesSettings settings,
            ILogger logger)
            : base(settings, logger)
        { }

        public new PaginatedPagesSettings GetSettings => this.Settings as PaginatedPagesSettings;

        public event EventHandler<PaginatedPageScrapedEventArgs> PaginatedPageScraped;

        public override async Task Scrape()
        {
            var document = await base.OpenDocument();
            var paginatedPage = this.ScrapePaginatedPage(
                document,
                this.GetSettings.ListItemsSelector,
                this.GetSettings.Properties);
            
        }

        public virtual PaginatedPage ScrapePaginatedPage(
            IDocument document, 
            string listItemsSelector, 
            IEnumerable<PropertySettings> properties)
        {
            var listProperties = base.ScrapeListProperties(
                document,
                this.GetSettings.ListItemsSelector,
                this.GetSettings.Properties);
            var pagination = this.ScrapePagination(
                document,
                this.GetSettings);

            var paginatedPage = new PaginatedPage
            {
                ListProperties = listProperties,
                Pagination = pagination
            };
            var e = new PaginatedPageScrapedEventArgs
            {
                Url = document.Url,
                PaginatedPage = paginatedPage
            };
            this.PaginatedPageScraped?.Invoke(this, e);
            return paginatedPage;
        }

        public virtual Pagination ScrapePagination(
            IDocument document,
            PaginatedPagesSettings paginatedSettings)
        {
            var pagination = new Pagination();
            if (!string.IsNullOrEmpty(paginatedSettings.TotalPagesSelector))
            {
                var totalPagesElement = document.QuerySelector(paginatedSettings.TotalPagesSelector);
                pagination.TotalPages = this.ResolveTotalPages(totalPagesElement);
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.FirstPage = document.QuerySelector(paginatedSettings.FirstPageSelector) as IHtmlAnchorElement;
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.PreviousPage = document.QuerySelector(paginatedSettings.PreviousPageSelector) as IHtmlAnchorElement;
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.Pages = document.QuerySelectorAll(paginatedSettings.PagesSelector) as IEnumerable<IHtmlAnchorElement>;
            }

            if (!string.IsNullOrEmpty(paginatedSettings.CurrentPage))
            {
                pagination.CurrentPage = document.QuerySelector(paginatedSettings.CurrentPage);
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.NextPage = document.QuerySelector(paginatedSettings.NextPageSelector) as IHtmlAnchorElement;
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.LastPage = document.QuerySelector(paginatedSettings.LastPageSelector) as IHtmlAnchorElement;
            }

            return pagination;
        }

        public virtual int? ResolveTotalPages(IElement totalPagesElement)
        {
            if (int.TryParse(totalPagesElement.TextContent, out int totalPages))
            {
                return totalPages;
            }

            return null;
        }
    }
}