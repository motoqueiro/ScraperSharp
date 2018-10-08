namespace ScraperSharp.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using ScraperSharp.Entities;
    using ScraperSharp.Scrapers;
    using ScraperSharp.Settings;
    using Xunit;

    public class ListPageUnitTests
        : BaseUnitTests
    {
        [Fact]
        public void ListPageScraper_NullListItemsSelector_ThrowsException()
        {
            //Arrange
            var listPageSettings = this.GetListPageSettings();
            listPageSettings.ListItemsSelector = null;

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new ListPageScraper(
                listPageSettings,
                this._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("ListItemsSelector");
        }

        [Fact]
        public async Task ListPageScraper_ShouldBeOk()
        {
            //Arrange
            var listPageSettings = this.GetListPageSettings();
            var scraper = new ListPageScraper(
                listPageSettings,
                this._loggerMock.Object);
            scraper.BeforeDocumentOpen += (sender, e) =>
            {
                sender.Should().NotBeNull();
                e.BrowsingContext.Should().NotBeNull();
                e.Url.Should().NotBeNullOrEmpty();
            };
            scraper.AfterDocumentOpen += (sender, e) =>
            {
                sender.Should().NotBeNull();
                e.Document.Should().NotBeNull();
            };
            scraper.PropertyScraped += (sender, e) =>
            {
                sender.Should().NotBeNull();
                AssertProperty(e.ScrapedProperty, e.Settings);
            };
            scraper.ListPageScraped += (sender, e) =>
            {
                sender.Should().NotBeNull();
                e.Settings.Should().Be(listPageSettings);
                e.Url.Should().NotBeNullOrEmpty();
                AssertListProperties(e.ListProperties, e.Settings.Properties);
            };
            scraper.PageScraped += (sender, e) =>
            {
                sender.Should().NotBeNull();
                e.Url.Should().NotBeNullOrEmpty();
                e.Settings.Should().Be(listPageSettings);
                this.AssertProperties(
                    e.Properties.ToList(),
                    listPageSettings.Properties);
            };

            //Act
            await scraper.Scrape();
        }

        [Fact]
        public async Task Test()
        {
            var settings = new ListPageSettings
            {
                Url = "https://www.fppadel.pt/jogadores-e-praticantes",
                ListItemsSelector = "div#texto > div.tblCell",
                ItemProperties = new List<PropertySettings>
                {
                    new PropertySettings
                    {
                        Name = "Image",
                        Selector = "span:nth-child(1) > span"
                    },
                    new PropertySettings
                    {
                        Name = "Number",
                        Selector = "span:nth-child(2)"
                    },
                    new PropertySettings
                    {
                        Name = "Name",
                        Selector = "span:nth-child(3)"
                    },
                    new PropertySettings
                    {
                        Name = "Gender",
                        Selector = "span:nth-child(4)"
                    },
                    new PropertySettings
                    {
                        Name = "Club",
                        Selector = "span:nth-child(5)"
                    },
                    new PropertySettings
                    {
                        Name = "ValidLicenseYear",
                        Selector = "span:nth-child(6)"
                    }
                }                
            };
            var scraper = new ListPageScraper(
                settings,
                this._loggerMock.Object);
            scraper.PropertyScraped += (sender, e) =>
            {
                
            };
            scraper.ListPageScraped += (sender, e) =>
            {

            };

            await scraper.Scrape();
        }

        private void AssertListProperties(
            IDictionary<int, IEnumerable<Property>> dictionaryProperties, 
            IList<PropertySettings> propertiesSettings)
        {
            foreach (var propertyPair in dictionaryProperties)
            {
                propertyPair.Key.Should().BeGreaterOrEqualTo(0);
                AssertProperties(
                    propertyPair.Value,
                    propertiesSettings);
            }
        }
    }
}