using AngleSharp.Dom;

namespace ScraperSharp.Entities
{
    public class Property
    {
        public string Name { get; set; }

        public IElement Element { get; set; }
    }
}