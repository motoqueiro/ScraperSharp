namespace ScraperSharp.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using ScraperSharp.Entities;
    using ScraperSharp.Settings;

    public abstract class BaseUnitTests
    {
        internal readonly Mock<ILogger> _loggerMock;

        public BaseUnitTests()
        {
            this._loggerMock = new Mock<ILogger>();
        }

        internal ListPageSettings GetListPageSettings()
        {
            return new ListPageSettings
            {
                Url = "https://www.fppadel.pt/competicoes/calendario/2018",
                ListItemsSelector = "div.cal-prova",
                Properties = new List<PropertySettings>
                {
                    new PropertySettings{
                        Name = "Cartaz",
                        Selector = "div.cal-prova-img > img"
                    },
                    new PropertySettings{
                        Name = "Título",
                        Selector = "div.cal-prova-info > a.linkTexto"
                    },
                    new PropertySettings{
                        Name = "Data",
                        Selector = "div.cal-prova-info > span[title='Data']"
                    },
                    new PropertySettings{
                        Name = "Local",
                        Selector = "div.cal-prova-info > span[title='Local']"
                    },
                    new PropertySettings{
                        Name = "Organização",
                        Selector = "div.cal-prova-info > em[title='Organização']"
                    }
                }
            };
        }

        internal PageSettings GetPageSettings()
        {
            return new PageSettings
            {
                Url = "https://moz.com/community/users/63",
                Properties = new List<PropertySettings> {
                    new PropertySettings{
                        Name = "Name",
                        Selector = "h4.name"
                    },
                    new PropertySettings{
                        Name = "Ranking",
                        Selector = "div.tag-standard"
                    },
                    new PropertySettings{
                        Name = "Categories",
                        Selector = "span.post-cats"
                    },
                    new PropertySettings{
                        Name = "Description",
                        Selector = "div.entry > p"
                    },
                    new PropertySettings{
                        Name = "",
                        Selector = ""
                    },
                    new PropertySettings{
                        Name = "",
                        Selector = ""
                    },
                    new PropertySettings{
                        Name = "",
                        Selector = ""
                    },
                    new PropertySettings{
                        Name = "",
                        Selector = ""
                    },
                    new PropertySettings{
                        Name = "",
                        Selector = ""
                    },
                }
            };
        }

        internal void AssertProperties(
            IEnumerable<Property> properties,
            IEnumerable<PropertySettings> expectedPropertySettings)
        {
            for (int index = 0; index < expectedPropertySettings.Count(); index++)
            {
                this.AssertProperty(
                    properties.ElementAt(index),
                    expectedPropertySettings.ElementAtOrDefault(index));
            }
        }

        internal void AssertProperty(
            Property property,
            PropertySettings expectedPropertySettings)
        {
            property.Should().NotBeNull();
            expectedPropertySettings.Should().NotBeNull();
            property.Name.Should().Be(expectedPropertySettings.Name);
            property.Element.Should().NotBeNull();
        }
    }
}