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

    [Trait("Category", "Page Scraper")]
    public class PageScraperUnitTests
        : BaseUnitTests
    {
        [Fact]
        public void PageScraper_NullSettings_ThrowException()
        {
            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                null,
                this._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("settings");
        }

        [Fact]
        public void PageScraper_NullUrl_ThrowsException()
        {
            //Arrange
            var pageSettings = this.GetPageSettings();
            pageSettings.Url = null;

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                pageSettings,
                this._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("Url");
        }

        [Fact]
        public void PageScraper_NullProperties_ThrowsException()
        {
            //Arrange
            var pageSettings = this.GetPageSettings();
            pageSettings.Properties = null;

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                pageSettings,
                this._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("Properties");
        }

        [Fact]
        public void PageScraper_NullProperty_ThrowsException()
        {
            //Arrange
            var pageSettings = this.GetPageSettings();
            pageSettings.Properties.Add(null);

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                pageSettings,
                this._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("The argument PropertySettings with index 1 is null!");
        }

        [Fact]
        public void PageScraper_NullPropertySettingsName_ThrowsException()
        {
            //Arrange
            var pageSettings = this.GetPageSettings();
            pageSettings.Properties.Add(new PropertySettings
            {
                Name = null,
                Selector = "a"
            });

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                pageSettings,
                this._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("The Name property of argument PropertySettings with index 1 is null!");
        }

        [Fact]
        public void PageScraper_NullPropertySettingsSelector_ThrowsException()
        {
            //Arrange
            var pageSettings = this.GetPageSettings();
            pageSettings.Properties.Add(new PropertySettings
            {
                Name = "Test",
                Selector = null
            });

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                pageSettings,
                this._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("The Selector property of argument PropertySettings with index 1 is null!");
        }

        [Fact]
        public void PageScraper_NullLogger_ThrowException()
        {
            //Arrange
            var pageSettings = this.GetPageSettings();

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                pageSettings,
                null));

            //Assert
            exception.Message.Should().Contain("logger");
        }

        [Fact]
        public async Task DetailPageScraper_ShouldBeOk()
        {
            //Arrange
            var pageSettings = this.GetPageSettings();
            var scraper = new PageScraper(
                pageSettings,
                this._loggerMock.Object);
            scraper.BeforeDocumentOpen += (sender, e) =>
            {
                e.BrowsingContext.Should().NotBeNull();
                e.Url.Should().NotBeNullOrEmpty();
            };
            scraper.AfterDocumentOpen += (sender, e) =>
            {
                e.Document.Should().NotBeNull();
            };
            scraper.PropertyScraped += (sender, e) =>
            {
                e.ScrapedProperty.Name.Should().Be("Title");
                e.ScrapedProperty.Element.Should().NotBeNull();
                e.ScrapedProperty.Element.TextContent.Should().Be("Tomorrowland – Official Aftermovie – 30-JUL-2018");
                e.Settings.Should().Equals(pageSettings.Properties);
            };
            scraper.PageScraped += (sender, e) =>
            {
                e.Url.Should().NotBeNullOrEmpty();
                e.Settings.Should().Equals(pageSettings.Properties);
                this.AssertProperties(
                    e.Properties.ToList(),
                    pageSettings.Properties);
            };

            //Act
            await scraper.Scrape();
        }
    }
}