namespace ScraperSharp.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public event EventHandler<PaginatedPagesScrapedEventArgs> PaginatedPagesScraped;

        public override async Task Scrape()
        {
            var pages = await this.ScrapePaginatedPages();
            var e = new PaginatedPagesScrapedEventArgs
            {
                Settings = this.GetSettings,
                Pages = pages
            };
            this.PaginatedPagesScraped?.Invoke(
                this,
                e);
        }

        public virtual async Task<IEnumerable<PaginatedPage>> ScrapePaginatedPages()
        {
            var paginatedPage = null as PaginatedPage;
            var pages = new List<PaginatedPage>();
            do
            {
                var url = this.ResolveUrl(paginatedPage?.Pagination);
                var document = await base.OpenDocument(url);
                paginatedPage = this.ScrapePaginatedPage(
                    document,
                    this.GetSettings.ListItemsSelector,
                    this.GetSettings.Properties);
                pages.Add(paginatedPage);
            } while (this.ResolveNextPageCondition(paginatedPage));

            return pages;
        }

        public virtual bool ResolveNextPageCondition(PaginatedPage paginatedPage)
        {
            if (paginatedPage.Pagination.HasNextPage)
            {
                return true;
            }
            var currentPage = this.ResolveCurrentPage(paginatedPage.Pagination.CurrentPage);
            if (paginatedPage.Pagination.HasPage(currentPage))
            {
                return true;
            }

            return false;
        }

        public virtual string ResolveUrl(Pagination pagination = null)
        {
            if (pagination == null)
            {
                return this.GetUrl();
            }

            IHtmlAnchorElement nextPageAnchor = null;
            if (pagination.HasNextPage)
            {
                nextPageAnchor = pagination.NextPage as IHtmlAnchorElement;
                return nextPageAnchor?.Href;
            }

            var currentPage = this.ResolveCurrentPage(pagination.CurrentPage);
            if (pagination.HasPage(currentPage))
            {
                nextPageAnchor = pagination.Pages[currentPage] as IHtmlAnchorElement;
                return nextPageAnchor?.Href;
            }

            return null;
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
                Url = document.Url,
                ListProperties = listProperties,
                Pagination = pagination
            };
            var e = new PaginatedPageScrapedEventArgs
            {
                Url = document.Url,
                Settings = this.GetSettings,
                PaginatedPage = paginatedPage
            };
            this.PaginatedPageScraped?.Invoke(this, e);
            return paginatedPage;
        }

        public virtual Pagination ScrapePagination(
            IDocument document,
            PaginatedPagesSettings paginatedSettings)
        {
            var pagination = new Pagination
            {
                TotalPages = document.QuerySelector(paginatedSettings.TotalPagesSelector),
                CurrentPage = document.QuerySelector(paginatedSettings.CurrentPageSelector)
            };

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.FirstPage = document.QuerySelector(paginatedSettings.FirstPageSelector);
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.PreviousPage = document.QuerySelector(paginatedSettings.PreviousPageSelector);
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.Pages = document.QuerySelectorAll(paginatedSettings.PagesSelector).ToList();
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.NextPage = document.QuerySelector(paginatedSettings.NextPageSelector);
            }

            if (!string.IsNullOrEmpty(paginatedSettings.FirstPageSelector))
            {
                pagination.LastPage = document.QuerySelector(paginatedSettings.LastPageSelector);
            }

            return pagination;
        }

        public virtual int ResolveTotalPages(IElement totalPagesElement)
        {
            return this.ResolveNumericValue(totalPagesElement);
        }

        public virtual int ResolveCurrentPage(IElement currentPageElement)
        {
            return this.ResolveNumericValue(currentPageElement);
        }

        public virtual int ResolveNumericValue(IElement element)
        {
            if (element != null
                && int.TryParse(element.TextContent, out int value))
            {
                return value;
            }

            return default(int);
        }

        internal override void ValidateSettings(PageSettings settings)
        {
            base.ValidateSettings(settings);
            var paginatedPagesSettings = settings as PaginatedPagesSettings;
            if (string.IsNullOrEmpty(paginatedPagesSettings.CurrentPageSelector))
            {
                throw new ArgumentNullException(nameof(paginatedPagesSettings.CurrentPageSelector));
            }
        }
    }
}