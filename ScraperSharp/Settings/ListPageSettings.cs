namespace ScraperSharp.Settings
{
    using System.Collections.Generic;

    public class ListPageSettings
        : PageSettings
    {
        public string ListItemsSelector { get; set; }

        public IList<PropertySettings> ItemProperties { get; set; }
    }
}