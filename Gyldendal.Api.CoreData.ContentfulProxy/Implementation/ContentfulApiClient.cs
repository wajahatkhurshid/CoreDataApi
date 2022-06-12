using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.ContentfulProxy.Model;

namespace Gyldendal.Api.CoreData.ContentfulProxy.Implementation
{
    public class ContentfulApiClient : BaseApiClient, IContentfulApiClient
    {
        public ContentfulApiClient(string spaceId, string environment, string previewApiKey, string deliveryApiKey) : base(spaceId, environment, previewApiKey, deliveryApiKey)
        {
        }

        public async Task<Author> GetAuthor(string id)
        {
            var builder = QueryBuilder<Author>.New.ContentTypeIs("author").FieldEquals(f => f.Id, id);
            var result = await ContentfulClient.GetEntries(builder);
            var author = result.FirstOrDefault();

            return author;
        }

        public async Task<Author> GetAuthorByAssetIdAsync(string id)
        {
            var builder = QueryBuilder<Author>.New.ContentTypeIs("author").FieldEquals(f => f.Image.Sys.Id, id);
            var result = await ContentfulClient.GetEntries(builder);
            var author = result.FirstOrDefault();

            return author;
        }

        public async Task<string> GetImageUrl(string imageId)
        {
            var result = await ContentfulClient.GetAsset(imageId);
            return result?.File.Url;
        }
    }
}
