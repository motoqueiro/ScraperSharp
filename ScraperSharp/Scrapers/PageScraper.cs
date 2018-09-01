namespace ScraperSharp.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AngleSharp;
    using AngleSharp.Dom;
    using Microsoft.Extensions.Logging;
    using ScraperSharp.Entities;
    using ScraperSharp.Events;
    using ScraperSharp.Settings;

    public class PageScraper
    {
        public PageScraper(
            PageSettings settings,
            ILogger logger)
        {
            this.Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var configuration = this.GetConfiguration();
            this.Context = BrowsingContext.New(configuration);
        }

        public PageSettings Settings { get; }

        public ILogger Logger { get; }

        public IBrowsingContext Context { get; private set; }

        public event EventHandler<BeforeDocumentOpenEventArgs> BeforeDocumentOpen;

        public event EventHandler AfterDocumentOpen;

        public event EventHandler<PropertyScrapedEventArgs> PropertyScraped;

        public event EventHandler<PageScrapedEventArgs> DetailPageScraped;

        public virtual async Task Scrape()
        {
            var document = await this.OpenDocument();
            var properties = this.ScrapeProperties(
                document.Body,
                this.Settings.Properties);
            var e = new PageScrapedEventArgs
            {
                Url = document.Url,
                Properties = properties
            };
            this.DetailPageScraped?.Invoke(this, e);
        }

        public virtual async Task<IDocument> OpenDocument()
        {
            var url = this.GetUrl();
            this.BeforeDocumentOpen?.Invoke(this, new BeforeDocumentOpenEventArgs { Url = url });
            var document = await this.Context.OpenAsync(url);
            this.AfterDocumentOpen?.Invoke(this, new AfterDocumentOpenEventArgs { Document = document });
            return document;
        }

        public virtual IEnumerable<Property> ScrapeProperties(
            IElement element,
            IEnumerable<PropertySettings> propertySettings)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (propertySettings == null)
            {
                throw new ArgumentNullException(nameof(propertySettings));
            }

            var properties = new List<Property>();
            foreach (var propertySetting in propertySettings)
            {
                var property = this.ScrapeProperty(
                    element,
                    propertySetting);

                if (property == null)
                {
                    continue;
                }

                properties.Add(property);
            }

            return properties;
        }

        public virtual Property ScrapeProperty(
            IElement element,
            PropertySettings propertySettings)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (propertySettings == null)
            {
                throw new ArgumentNullException(nameof(propertySettings));
            }

            var propertyElement = element.QuerySelector(propertySettings.Selector);
            if (propertyElement == null)
            {
                return null;
            }

            var property = new Property
            {
                Name = propertySettings.Name
            };
            if (!string.IsNullOrEmpty(propertySettings.Attribute))
            {
                property.Value = propertyElement.GetAttribute(propertySettings.Attribute);
            }
            else
            {
                property.Value = propertyElement.TextContent;
            }

            var e = new PropertyScrapedEventArgs
            {
                ScrapedProperty = property
            };
            this.PropertyScraped?.Invoke(this, e);
            return property;
        }

        public virtual string GetUrl() => this.Settings.Url;

        public virtual IConfiguration GetConfiguration() => Configuration.Default
            .WithDefaultLoader()
            .WithCookies();
    }
}