namespace ScraperSharp.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using ScraperSharp.Entities;
    using ScraperSharp.Scrapers;
    using ScraperSharp.Settings;
    using Xunit;

    public class PageScraperUnitTests
    {
        private readonly Mock<ILogger> _loggerMock;

        public PageScraperUnitTests()
        {
            this._loggerMock = new Mock<ILogger>();
        }

        [Fact]
        public void PageScraper_NUllSettings_ThrowException()
        {
            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                null,
                this._loggerMock.Object));

            //Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Contain("settings");
        }

        [Fact]
        public void PageScraper_NUllLogger_ThrowException()
        {
            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PageScraper(
                new PageSettings(),
                null));

            //Assert
            exception.Should().NotBeNull();
            exception.Message.Should().Contain("logger");
        }

        [Fact]
        public async Task DetailPageScraper_SingleProperty_ShouldBeOk()
        {
            //Arrange
            var property = (Property)null;
            var settings = new PageSettings
            {
                Url = "http://www.global-sets.com/tomorrowland-official-aftermovie-30-jul-2018/",
                Properties = new PropertySettings[] {
                    new PropertySettings{
                        Name = "Title",
                        Selector = "h1.post-title > span"
                    }
                }
            };
            var scraper = new PageScraper(
                settings,
                this._loggerMock.Object);
            scraper.PropertyScraped += (sender, e) =>
            {
                property = e.ScrapedProperty;
            };

            //Act
            await scraper.Scrape();

            //Assert
            property.Should().NotBeNull();
            property.Name.Should().Be("Title");
            property.Value.Should().Be("Tomorrowland – Official Aftermovie – 30-JUL-2018");
        }
    }
}