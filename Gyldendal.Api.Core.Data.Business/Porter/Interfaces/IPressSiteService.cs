using Gyldendal.APi.CoreData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IPressSiteService
    {
        Task<PressSiteProduct> GetBookAsync(string isbn);

        Task<ProductResponse> GetProductsAsync(BogForsiderRequset requset);

        Task<ForfatterResponse> GetAuthorsAsync(AuthorsRequest request);

        Task<List<PressSiteProduct>> GetNewsletterTitles(NewsletterRequest request);

        Task<ProductResponse> GetUpcomingTitles(UpcomingTitlesRequest request);


        Task<ForfatterfotoResponse> GetAuthorPhotosAsync(AuthorPhotosRequest request);

        Task<List<Forfatter>> GetAuthorsForImportAsync();

        Task<List<PressSiteProduct>> GetProductsForImport(ProductImportRequest request);

        Task<TypeAheadResponse> GetTypeAheadResultsAsync(TypeAheadRequest request);
    }
}
