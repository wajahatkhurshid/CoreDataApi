using System.Threading.Tasks;
using Gyldendal.Api.CoreData.ContentfulProxy.Model;

namespace Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure
{
    public interface IContentfulManager
    {
        Task<string> GetContributorHeroImage(string contributorId);

        Task<Author> GetAuthorByAssetIdAsync(string assetId);
    }
}
