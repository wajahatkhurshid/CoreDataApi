namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class ProductBasicData
    {
        public string DigitalProductLink { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Isbn { get; set; }
        public string Isbn10 { get; set; }

        public string MediaType { get; set; }

        public string Description { get; set; }
    }
}
