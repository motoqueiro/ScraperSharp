namespace ScraperSharp.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AngleSharp.Dom;
    using Microsoft.Extensions.Logging;
    using ScraperSharp.Entities;
    using ScraperSharp.Events;
    using ScraperSharp.Settings;

    public class ListPageScraper
        : PageScraper
    {
        public ListPageScraper(
            ListPageSettings settings,
            ILogger logger)
            : base(settings, logger)
        { }

        public ListPageSettings GetSettings => this.Settings as ListPageSettings;

        public event EventHandler<ListPageScrapedEventArgs> ListPageScraped;

        public override async Task Scrape()
        {
            var document = await this.OpenDocument();
            var listProperties = this.ScrapeListProperties(
                document,
                this.GetSettings.ListItemsSelector,
                this.GetSettings.Properties);
            var e = new ListPageScrapedEventArgs
            {
                Url = document.Url,
                ListProperties = listProperties
            };
            this.ListPageScraped?.Invoke(this, e);
        }

        public virtual IDictionary<int, IEnumerable<Property>> ScrapeListProperties(
            IDocument document,
            string listItemsSelector,
            IEnumerable<PropertySettings> propertySettings)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (string.IsNullOrEmpty(listItemsSelector))
            {
                throw new ArgumentNullException(nameof(listItemsSelector));
            }

            if (propertySettings == null)
            {
                throw new ArgumentNullException(nameof(propertySettings));
            }

            var listItemElements = document.QuerySelectorAll(listItemsSelector);
            if (listItemElements == null)
            {
                return null;
            }

            var listItems = new Dictionary<int, IEnumerable<Property>>();
            for (int i = 0; i < listItemElements.Length; i++)
            {
                var properties = this.ScrapeProperties(
                    listItemElements[i],
                    propertySettings);
                listItems.Add(i, properties);
            }

            return listItems;
        }
    }
}