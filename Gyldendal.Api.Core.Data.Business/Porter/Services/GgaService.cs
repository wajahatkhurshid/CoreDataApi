using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.APi.CoreData.Models;
using Gyldendal.Api.CoreData.Services.PorterApiClient;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class GgaService : IGgaService
    {
        private readonly IPorterClient _porterClient;
        private static readonly string[] AllowedGgaPublishers = { "gyldendal", "GB_Forlagene", "GyldendalAkademisk" };

        public GgaService(IPorterClient porterClient)
        {
            _porterClient = porterClient;
        }

        public async Task<SearchDtoResponse> Search(string criteria, int mode)
        {
            if (mode == (int)SearchType.Title) //Search Title with its Authors
            {
                // TODO: Illustrator and Translator need to be fetched
                var propsToInclude =
                    $"{nameof(Product.Isbn)},{nameof(Product.Title)} ,{nameof(Product.Edition)}, {nameof(Product.Publisher)},{nameof(Product.NoOfPages)},{nameof(Product.FirstPrintPublishDate)},{nameof(Product.Contributors)}";
                var result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery()
                {
                    Isbns = new List<string> { criteria },
                    PropertiesToInclude = propsToInclude,
                    WebShop = WebShop.All
                });

                return GetProduct(result.Results.FirstOrDefault(IsValidProductSearch));
            }
            else
            {
                var propsToInclude =
                    $"{nameof(Contributor.FirstName)},{nameof(Contributor.LastName)} ,{nameof(Contributor.Id)}, {nameof(Contributor.PhotoUrl)}";
                var result = await _porterClient.ApiV1ContributorContributorsearchAsync(new ContributorSearchQuery()
                {
                    SearchString = criteria,
                    PropertiesToInclude = propsToInclude
                });
                return GetAuthor(result.Results);
            }
        }

        private bool IsValidProductSearch(Product product)
        {
            return AllowedGgaPublishers.Contains(product.Publisher);
        }

        private SearchDtoResponse GetProduct(Product product)
        {
            return new()
            {
                ISBN = product.Isbn,
                TitleName = product.Title,
                EditionNumber = product.Edition ?? 0,
                OriginalPublisher = product.Publisher,
                Pages = product.NoOfPages.ToString(),
                PublicationDate = product.FirstPrintPublishDate ?? DateTime.MinValue
            };
        }

        private SearchDtoResponse GetAuthor(IEnumerable<Contributor> contributor)
        {
            return new()
            {
                Authors = contributor.Select(x =>
                {
                    var photoUrl = string.IsNullOrWhiteSpace(x.PhotoUrl)
                        ? "http://ifilserver.gyldendal.dk/portraets/missingfotogdk.jpg"
                        : x.PhotoUrl;

                    return new AuthorResponse
                    {
                        AuthorName = x.FirstName + " " + x.LastName,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        AuthorId = x.Id,
                        AuthorUrlBig = photoUrl,
                        AuthorUrlMed = photoUrl,
                        AuthorUrlSmall = photoUrl
                    };
                }).ToList()
            };
        }
    }
}