using System.Threading.Tasks;
using Gyldendal.Api.CoreData.ContentfulProxy.Model;

namespace Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure
{
    public interface IContentfulApiClient
    {
        Task<Author> GetAuthor(string id);

        Task<Author> GetAuthorByAssetIdAsync(string id);

        Task<string> GetImageUrl(string imageId);
    }
}
