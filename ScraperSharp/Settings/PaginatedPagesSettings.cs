namespace ScraperSharp.Settings
{
    public class PaginatedPagesSettings
        : ListPageSettings
    {
        public string TotalPagesSelector { get; set; }

        public string FirstPageSelector { get; set; }

        public string PreviousPageSelector { get; set; }

        public string PagesSelector { get; set; }

        public string CurrentPageSelector { get; set; }

        public string NextPageSelector { get; set; }

        public string LastPageSelector { get; set; }
    }
}