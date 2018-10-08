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

        public virtual string GetListItemsSelector => this.GetSettings?.ListItemsSelector;

        public virtual IEnumerable<PropertySettings> GetItemProperties => this.GetSettings?.ItemProperties;

        public event EventHandler<ListPageScrapedEventArgs> ListPageScraped;

        public override async Task Scrape()
        {
            var document = await this.OpenDocument(this.GetSettings.Url);
            var e = new ListPageScrapedEventArgs
            {
                Url = document.Url,
                Settings = this.GetSettings
            };
            if (this.GetSettings.Properties != null)
            {
                e.Properties = this.ScrapeProperties(
                        document.Body,
                        this.GetSettings.Properties);
            }

            if (this.GetItemProperties != null)
            {
                e.ListProperties = this.ScrapeListProperties(
                        document,
                        this.GetListItemsSelector,
                        this.GetItemProperties);
            }

            this.ListPageScraped?.Invoke(this, e);
        }

        public virtual IDictionary<int, IEnumerable<Property>> ScrapeListProperties(
            IDocument document,
            string listItemsSelector,
            IEnumerable<PropertySettings> propertySettings)
        {
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

        internal override void ValidateSettings(PageSettings settings)
        {
            var listPageSettings = settings as ListPageSettings;
            if (string.IsNullOrEmpty(listPageSettings.ListItemsSelector))
            {
                throw new ArgumentNullException(nameof(listPageSettings.ListItemsSelector));
            }

            base.ValidatePropertySettings(listPageSettings.ItemProperties);
            base.ValidateSettings(settings);
        }
    }
}