using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories
{
    public interface IProductFactory
    {
        Product GetProductByIsbn(DataScope dataScope, string id, ProductType productType);

        int GetUpdatedProductsCount(DataScope dataScope, DateTime updatedAfter);

        IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DataScope dataScope, DateTime updatedAfter, int pageIndex, int pageSize);

        Result<string> GetBundleIdsByIsbn(DataScope dataScope, string isbn, int pageIndex, int pageSize);

        IEnumerable<string> GetCampaignProducts(DataScope dataScope, DateTime updatedAfterTicks, int pageIndex, int pageSize);

        int GetCampaignProductsCount(DataScope dataScope, DateTime updatedAfterTicks);

        bool HasActiveCampaign(DataScope dataScope, string productId);

        bool IsProductBuyable(DataScope dataScope, Product product);

        bool IsProductBuyable(DataScope dataScope, BundleProduct bundleProduct);

        Dictionary<string, List<WebShop>> GetProductWebshops(DataScope dataScope, List<string> isbns);
    }
}