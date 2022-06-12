using Gyldendal.APi.CoreData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Common.DataProviderInfrastructure
{
    public interface IPressSiteProvider
    {
        PressSiteProduct GetBook(string Isbn13);
        ForfatterResponse GetAuthors(AuthorsRequest request);
        ForfatterfotoResponse GetAuthorPhotos(AuthorPhotosRequest request);
        ProductResponse GetBookCovers(BogForsiderRequset requset);
        List<PressSiteProduct> GetNewsletterTitles(NewsletterRequest request);
        ProductResponse GetUpcomingTitles(UpcomingTitlesRequest requset);
        List<string> GetCategories();
        List<PressSiteProduct> GetProductsForImport(ProductImportRequest request);
        List<Forfatter> GetAuthorsForImport();
        TypeAheadResponse GetTypeAheadResults(TypeAheadRequest request);
    }
}
