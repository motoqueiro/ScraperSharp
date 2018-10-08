namespace ScraperSharp.Settings
{
    using System.Collections.Generic;

    public class PageSettings
    {
        public string Url { get; set; }

        public IList<PropertySettings> Properties { get; set; }
    }
}