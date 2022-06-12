using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface IProductRepository : ICoreDataRepository
    {
        Product GetProductByIsbn(string isbn);

        /// <summary>
        /// Get GU Bundle details By isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        Product GetBundleByIsbn(string isbn);

        int GetUpdatedProductsCount(DateTime updatedAfter);

        IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DateTime updatedAfter, int pageIndex, int pageSize);

        Result<string> GetBundleIdsByIsbn(string isbn, int pageIndex, int pageSize);

        IEnumerable<string> GetCampaignProducts(DateTime updatedAfter, int pageIndex, int pageSize);

        int GetCampaignProductsCount(DateTime updatedAfter);

        bool HasActiveCampaign(string productId);

        bool IsProductBuyable(Product product);

        bool IsProductBuyable(BundleProduct bundleProduct);

        Dictionary<string, List<WebShop>> GetProductWebshops(List<string> isbns);
    }
}