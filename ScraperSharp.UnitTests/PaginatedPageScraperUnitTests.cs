namespace ScraperSharp.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using ScraperSharp.Scrapers;
    using ScraperSharp.Settings;
    using Xunit;

    [Trait("Category", "Paginated Page Scraper")]
    public class PaginatedPageScraperUnitTests
        : BaseUnitTests
    {
        [Fact]
        public void PaginatedPageScraper_NullCurrentPageSelector_ThrowsException()
        {
            //Arrange
            var paginatedPageSettings = this.GetPageSettings() as PaginatedPagesSettings;

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => new PaginatedPagesScraper(
                paginatedPageSettings,
                base._loggerMock.Object));

            //Assert
            exception.Message.Should().Contain("CurrentPageSelector");
        }

        [Fact]
        public async Task PaginatedPageScraper_ShouldBeOk()
        {
            //Arrange
            var settings = new PaginatedPagesSettings
            {
                FirstPageSelector = "a.page_first",
                PreviousPageSelector = "a.page_prev",
                PagesSelector = "span.pagenrs > a",
                CurrentPageSelector = "span.page_selected",
                NextPageSelector = "a.page_next",
                LastPageSelector = "a.page_last",
                Url = "http://te.tournamentsoftware.com/ranking/category.aspx?id=18031&category=516&ogid=C1DBDB91-4E25-4D36-AD94-B404E369E50F&C516FOG=&p=1&ps=10",
                ListItemsSelector = "table.ruler > tbody > tr:not(:first-child):not(:last-child)",
                TotalPagesSelector = "span.page_caption",
                Properties = new[] {
                    new PropertySettings{
                        Name = "Ranking",
                        Selector = "td.rank:nth-child(1)"
                    },
                    new PropertySettings{
                        Name = "Player",
                        Selector = "td:nth-child(4) > a"
                    },
                    new PropertySettings{
                        Name = "YearOfBirth",
                        Selector = "td:nth-child(5)"
                    },
                    new PropertySettings{
                        Name = "Points",
                        Selector = "td:nth-child(6)"
                    },
                    new PropertySettings{
                        Name = "TotalPoints",
                        Selector = "td:nth-child(9)"
                    },
                    new PropertySettings{
                        Name = "Tournaments",
                        Selector = "td:nth-child(10)"
                    },
                    new PropertySettings{
                        Name = "Country",
                        Selector = "td:nth-child(11) > a"
                    }
                }
            };

            //Act
            var scraper = new PaginatedPagesScraper(
                settings,
                this._loggerMock.Object);
            scraper.BeforeDocumentOpen += (sender, e) =>
            {
                sender.Should().BeOfType<PaginatedPagesScraper>();
                e.Url.Should().NotBeNullOrEmpty();
            };
            scraper.AfterDocumentOpen += (sender, e) =>
            {
                sender.Should().BeOfType<PaginatedPagesScraper>();
                e.Document.Should().NotBeNull();
            };
            scraper.ListPageScraped += (sender, e) =>
            {
                sender.Should().BeOfType<PaginatedPagesScraper>();
                e.Url.Should().NotBeNullOrEmpty();
                e.Settings.Should().NotBeNull();
                e.ListProperties.Should().NotBeNull();
            };
            scraper.PropertyScraped += (sender, e) =>
            {
                sender.Should().BeOfType<PaginatedPagesScraper>();
                e.ScrapedProperty.Should().NotBeNull();
                e.ScrapedProperty.Name.Should().NotBeNullOrEmpty();
                e.ScrapedProperty.Element.Should().NotBeNull();
                e.Settings.Should().NotBeNull();
            };

            await scraper.Scrape();
        }
    }
}