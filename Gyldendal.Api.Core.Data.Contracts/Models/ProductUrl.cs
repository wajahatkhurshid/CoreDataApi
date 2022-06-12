using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Models
{
    public class ProductUrl
    {
        public ProductUrlType UrlType { get; set; }

        public string Url { get; set; }
    }
}
