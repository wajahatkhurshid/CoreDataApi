using Contentful.Core.Models;

namespace Gyldendal.Api.CoreData.ContentfulProxy.Model
{
    public class Author
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Contentful.Core.Models.Management.Reference Image { get; set; }

        public Document Description { get; set; }
    }
}
