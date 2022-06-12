using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByIsbnAsync(WebShop webShop, string id, ProductType productType);
        Task<ProductAccessControlType> GetProductAccessTypeByIsbnAsync(string id);
        Task<List<ProductBasicData>> GetLicensedProductsByIsbnAsync(WebShop webshop, IList<string> isbns, bool withImageUrl);
        Task<int> GetUpdatedProductsCountAsync(WebShop webshop, long updateAfterTicks);
        Task<List<ProductUpdateInfo>> GetUpdatedProductsInfoAsync(WebShop webshop, long updateAfterTicks, int pageIndex, int pageSize);
        Task<string> GetSupplementaryDataAsync(string isbn);
        IEnumerable<string> GetCampaignProducts(WebShop webShop, DateTime updatedAfterTicks, int pageIndex,
            int pageSize);
        int GetCampaignProductsCount(WebShop webShop, DateTime updatedAfterTicks);
        IEnumerable<string> GetUpdatedBundleCampaignsInfo(WebShop webShop, DateTime updatedAfterTicks, int pageIndex,
            int pageSize);
        int GetBundleCampaignsCount(WebShop webShop, DateTime updatedAfterTicks);

        Task<bool> IsProductDataAvailable();

        Task<Dictionary<string, List<WebShop>>> GetProductWebshopsAsync(List<string> isbns);
    }
}
