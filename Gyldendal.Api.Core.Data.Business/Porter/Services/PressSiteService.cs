using Gyldendal.Api.CoreData.Services.PorterApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.APi.CoreData.Models;

namespace Gyldendal.Api.CoreData.Business.Porter.Services
{
    public class PressSiteService : IPressSiteService
    {
        private readonly IPorterClient _porterClient;
        public PressSiteService(IPorterClient porterClient)
        {
            _porterClient = porterClient;
        }

        public async Task<PressSiteProduct> GetBookAsync(string isbn)
        {
            var result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery()
            {
                Isbn = isbn,
                PageIndex = 1,
                PageSize = 1,
                WebShop = WebShop.Pressesite
            });
            return GetProduct(result.Results.FirstOrDefault());
        }

        public async Task<ProductResponse> GetProductsAsync(BogForsiderRequset requset)
        {

            var result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery()
            {
                Isbn = requset.SearchString,
                PageIndex = requset.Page,
                PageSize = requset.PageSize,
                WebShop = WebShop.Pressesite,
                DateFrom = requset.DateFrom,
                DateTo = requset.DateTo
            });

            if (result == null)
            {
                result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery()
                {
                    Title = requset.SearchString,
                    PageIndex = requset.Page,
                    PageSize = requset.PageSize,
                    WebShop = WebShop.Pressesite,
                    DateFrom = requset.DateFrom,
                    DateTo = requset.DateTo
                });

                return new ProductResponse()
                {
                    Count = (int)result.TotalResults,
                    Products = result.Results.Select(GetProduct).ToList()
                };
            }

            return new ProductResponse()
            {
                Count = (int)result.TotalResults,
                Products = result.Results.Select(GetProduct).ToList()
            };

        }

        public async Task<ForfatterResponse> GetAuthorsAsync(AuthorsRequest request)
        {
            var result = await _porterClient.ApiV1ContributorContributorsearchAsync(new ContributorSearchQuery()
            {
                SearchString = request.SearchString,
                Page = request.Page,
                PageSize = request.PageSize
            });
            ForfatterResponse forfatterResponse = new()
            {
                Forfatter = result.Results.Select(x => new Forfatter()
                {
                    ForfatterNavn = x.FirstName + ' ' + x.LastName,
                    ForfatterProfileLink = x.PhotoUrl
                }).ToList()
            };
            forfatterResponse.Count = result.Count;
            return forfatterResponse;

        }
        public async Task<List<PressSiteProduct>> GetNewsletterTitles(NewsletterRequest request)
        {
            var result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery
            {
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                WebShop = WebShop.Pressesite
            });

            var response = result.Results.Select(GetProduct);

            return response.ToList();
        }

        public async Task<ProductResponse> GetUpcomingTitles(UpcomingTitlesRequest request)
        {
            var result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery
            {
                Isbn = request.SearchString,
                ProductSortByOption = GetOrderBy(request.SortExpresion),
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                WebShop = WebShop.Pressesite

            });

            if (result == null)
            {
                result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery
                {
                    Title = request.SearchString,
                    ProductSortByOption = GetOrderBy(request.SortExpresion),
                    WebShop = WebShop.Pressesite

                });
            }

            var response = result.Results.Select(GetProduct);

            return new ProductResponse
            {
                Products = response.ToList(),
                Count = (int)result.TotalResults
            };
        }

        private ProductSortByOptions GetOrderBy(string value)
        {
            switch (value)
            {
                case "titel":
                    return ProductSortByOptions.Title;
                case "medie":
                    return ProductSortByOptions.MediaType;
                case "genre":
                    return ProductSortByOptions.SubTitle;
                case "udgivelsesdato":
                    return ProductSortByOptions.PublishDate;
                default:
                    return ProductSortByOptions.Isbn;
            }

        }

        public async Task<ForfatterfotoResponse> GetAuthorPhotosAsync(AuthorPhotosRequest request)
        {
            var result = await _porterClient.ApiV1ContributorContributorsearchAsync(new ContributorSearchQuery()
            {
                SearchString = request.SearchString,
                Page = request.Page,
                PageSize = request.PageSize
            });
            ForfatterfotoResponse forfatterfotoResponse = new()
            {
                Forfatterfoto = result.Results.Select(x => new Forfatterfoto()
                    {
                        Forfatter = x.FirstName + ' ' + x.LastName,
                        ImageSource = x.PhotoUrl
                    })
                    .ToList()
            };
            forfatterfotoResponse.Count = result.Count;
            return forfatterfotoResponse;
        }

        public async Task<List<Forfatter>> GetAuthorsForImportAsync()
        {
            var result = await _porterClient.ApiV1ContributorContributorsearchAsync(new ContributorSearchQuery());
            List<Forfatter> forfatter = new();
            forfatter = result.Results.Select(x => new Forfatter()
                {
                    Id = x.Id,
                    ForfatterNavn = x.FirstName + ' ' + x.LastName,
                    Fornavn = x.FirstName,
                    Efternavn = x.LastName,

                })
                .ToList();
            return forfatter;
        }

        public async Task<List<PressSiteProduct>> GetProductsForImport(ProductImportRequest request)
        {
            var result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery()
            {
                DateFrom = request.DateFrom,
                DateTo = request.DateTo,
                WebShop = WebShop.Pressesite
            });
            var productList = result.Results.Select(x => new PressSiteProduct()
                {
                    Isbn13 = x.Isbn,
                    Titel = x.Title
                })
                .ToList();
            return productList;
        }
        public async Task<TypeAheadResponse> GetTypeAheadResultsAsync(TypeAheadRequest request)
        {
            var typeAheadResponse = new TypeAheadResponse();
            AuthorsRequest authorRequest = new()
            {
                SearchString = request.PrefixText,
                Page = 1,
                PageSize = 5,
                SortExpresion = "Forfatter"
            };
            var forfatters = await GetAuthorsAsync(authorRequest);
            typeAheadResponse.CountAuthors = forfatters.Count;
            typeAheadResponse.ListAuthors = forfatters.Forfatter.Select(x => new Forfatterfoto
                {
                    Forfatter = x.ForfatterNavn,
                    ImageSource = x.Photo?.ImageSource
                })
                .ToList();
            var titles = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery
            {
                Isbn = request.PrefixText,
                ProductSortByOption = GetOrderBy("Titel"),
                DateFrom = request.FromDateK,
                DateTo = request.ToDateK,
                PageIndex = 1,
                PageSize = 5,
                WebShop = WebShop.Pressesite
            });

            typeAheadResponse.CountKtitler = (int) titles.TotalResults;
            typeAheadResponse.ListKtitler = titles.Results.Select(x => new BogForsider
                {
                    Titel = x.Title,
                    // OstISBN = x.Isbn10,
                    //Organization = x.Organization,
                    ISBN13 = x.Isbn,
                    PublishDate = (DateTime) x.CurrentPrintRunPublishDate
                })
                .ToList();
            var result = await _porterClient.ProductApiV1ProductProductsearchAsync(new ProductSearchQuery
            {
                Isbn = request.PrefixText,
                DateFrom = request.FromDate,
                DateTo = request.ToDate,
                PageIndex = 1,
                PageSize = 5,
                WebShop = WebShop.Pressesite

            });
            typeAheadResponse.CountBooks = (int)result.TotalResults;
            typeAheadResponse.ListBooks = result.Results.Select(x => new BogForsider
                {
                    Titel = x.Title,
                    BookDetailLink = x.Isbn,
                    ISBN13 = x.Isbn,
                    //OstISBN = b.ostSBN,
                    //Organization = b.organisationname,
                    PublishDate = (DateTime) x.CurrentPrintRunPublishDate

                })
                .ToList();

            return typeAheadResponse;
        }
        private PressSiteProduct GetProduct(Product product)
        {
            //todo
            return new PressSiteProduct
            {
                VareId = product.Id,
                Titel = product.Title,
                PublishDate = product.CurrentPrintRunPublishDate,
                PublishedYear = product.CurrentPrintRunPublishDate == null ? "" : product.CurrentPrintRunPublishDate.Value.Year.ToString(),
                MediaType = product.MediaType,
                ImageSource = product.Url,
                ImageSize = 0,
                Genudg = product.Edition > 1 ? "Ja" : "-",
                Isbn10 = product.Isbn,
                Isbn13 = product.Isbn,
                Edition = product.Edition,
                NumberOfPages = product.NoOfPages.ToString(),
                // Oversatter = result.Oversaetter,
                //Omslagsgrafiker = result.omslagsgrafiker,
                Description = product.Description,
                //Kontaktperson = result.kontaktperson_navn,
                // KontaktpersonFone = result.kontaktperson_tlf,
                // Organization = result.organisationname,
                RetailPrice = Convert.ToDecimal(product.PriceWithoutVat)
            };
        }


    }
}
