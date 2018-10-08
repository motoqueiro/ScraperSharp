namespace ScraperSharp.Scrapers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.ValidateSettings(settings);
            this.Settings = settings;
            var configuration = this.GetConfiguration();
            this.Logger.LogWarning("Creating browsing context...");
            this.Context = BrowsingContext.New(configuration);
        }

        public PageSettings Settings { get; }

        public ILogger Logger { get; }

        public IBrowsingContext Context { get; private set; }

        public event EventHandler<BeforeDocumentOpenEventArgs> BeforeDocumentOpen;

        public event EventHandler<AfterDocumentOpenEventArgs> AfterDocumentOpen;

        public event EventHandler<PropertyScrapedEventArgs> PropertyScraped;

        public event EventHandler<PageScrapedEventArgs> PageScraped;

        public virtual async Task Scrape()
        {
            var url = this.GetUrl();
            var document = await this.OpenDocument(url);
            if (document == null)
            {
                this.Logger.LogWarning("Null document on url {0}...", url);
                return;
            }

            var properties = this.ScrapeProperties(
                document.Body,
                this.Settings.Properties);
            this.Logger.LogInformation("Sending event page scraped...");
            var e = new PageScrapedEventArgs
            {
                Url = document.Url,
                Settings = this.Settings,
                Properties = properties
            };
            this.PageScraped?.Invoke(this, e);
        }

        public virtual async Task<IDocument> OpenDocument(string url)
        {
            this.Logger.LogInformation("Sending event before document open...");
            this.BeforeDocumentOpen?.Invoke(this, new BeforeDocumentOpenEventArgs
            {
                Url = url,
                BrowsingContext = this.Context
            });
            this.Logger.LogInformation("Opening document on url {0}...", url);
            var document = await this.Context.OpenAsync(url);
            this.Logger.LogInformation("Sending event after document open...");
            this.AfterDocumentOpen?.Invoke(this, new AfterDocumentOpenEventArgs { Document = document });
            return document;
        }

        public virtual IEnumerable<Property> ScrapeProperties(
            IElement element,
            IEnumerable<PropertySettings> propertySettings)
        {
            this.Logger.LogInformation("Scraping properties...");
            var properties = new List<Property>();
            foreach (var propertySetting in propertySettings)
            {
                var property = this.ScrapeProperty(
                    element,
                    propertySetting);
                if (property == null)
                {
                    this.Logger.LogWarning("Null property {0}", propertySetting.Name);
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
            this.Logger.LogInformation(
                "Scraping property {0} with selector {1}...",
                propertySettings.Name,
                propertySettings.Selector);
            var property = new Property
            {
                Name = propertySettings.Name,
                Element = element.QuerySelector(propertySettings.Selector)
            };
            this.Logger.LogInformation("Sending event property scraped...");
            var e = new PropertyScrapedEventArgs
            {
                Settings = propertySettings,
                ScrapedProperty = property
            };
            this.PropertyScraped?.Invoke(this, e);
            return property;
        }

        public virtual string GetUrl()
        {
            this.Logger.LogWarning("Loading url...");
            return this.Settings.Url;
        }

        public virtual IConfiguration GetConfiguration()
        {
            this.Logger.LogInformation("Loading configuration...");
            return Configuration.Default
                .WithDefaultLoader()
                .WithCookies();
        }

        internal virtual void ValidateSettings(PageSettings settings)
        {
            this.Logger.LogInformation("Validating page settings...");
            if (settings == null)
            {
                this.Logger.LogError("Null page settings");
                throw new ArgumentNullException(nameof(settings));
            }

            if (string.IsNullOrEmpty(settings.Url))
            {
                this.Logger.LogError("Null or empty url");
                throw new ArgumentNullException(nameof(settings.Url));
            }

            this.ValidatePropertySettings(settings.Properties);
        }

        internal void ValidatePropertySettings(IList<PropertySettings> properties)
        {
            if (properties == null)
            {
                return;
            }

            this.Logger.LogInformation("Validating page property settings...");
            for (int index = 0; index < properties.Count(); index++)
            {
                this.ValidatePropertySettings(
                    properties[index],
                    index);
            }
        }

        internal void ValidatePropertySettings(
            PropertySettings propertySettings,
            int index)
        {
            if (propertySettings == null)
            {
                this.Logger.LogError("Null property settings on index {0}", index);
                throw new ArgumentNullException($"The argument PropertySettings with index {index} is null!");
            }

            if (string.IsNullOrEmpty(propertySettings.Name))
            {
                this.Logger.LogError("Null or empty property settings name on index {0}", index);
                throw new ArgumentNullException($"The Name property of argument PropertySettings with index {index} is null!");
            }

            if (string.IsNullOrEmpty(propertySettings.Selector))
            {
                this.Logger.LogError("Null or empty property settings selector on index {0}", index);
                throw new ArgumentNullException($"The Selector property of argument PropertySettings with index {index} is null!");
            }
        }
    }
}