using System.Threading.Tasks;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.ContentfulProxy.Model;

namespace Gyldendal.Api.CoreData.ContentfulProxy.Implementation
{
    public class ContentfulManager : IContentfulManager
    {
        private readonly IContentfulApiClient _contentfulApiClient;

        public ContentfulManager(IContentfulApiClient contentfulApiClient)
        {
            _contentfulApiClient = contentfulApiClient;
        }

        public async Task<string> GetContributorHeroImage(string contributorId)
        {
            var contentfulAuthor = await _contentfulApiClient.GetAuthor(contributorId);
            var imageUrl = string.Empty;

            if (contentfulAuthor?.Image != null)
            {
                imageUrl = await _contentfulApiClient.GetImageUrl(contentfulAuthor.Image.Sys.Id);
            }

            return imageUrl;
        }

        public async Task<Author> GetAuthorByAssetIdAsync(string assetId)
        {
            var author = await _contentfulApiClient.GetAuthorByAssetIdAsync(assetId);

            return author;
        }
    }
}
