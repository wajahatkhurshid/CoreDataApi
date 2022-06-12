using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Business.Repositories.StudyBox
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository() : base(DataScope.StudyBox, null)
        {
        }

        public Product GetProductByIsbn(string isbn)
        {
            throw GetNotImplementedException();
        }

        public Product GetBundleByIsbn(string isbn)
        {
            throw GetNotImplementedException();
        }

        public int GetUpdatedProductsCount(DateTime updatedAfter)
        {
            throw GetNotImplementedException();
        }

        public IEnumerable<ProductUpdateInfo> GetProductsUpdateInfo(DateTime updatedAfter, int pageIndex, int pageSize)
        {
            throw GetNotImplementedException();
        }

        public Result<string> GetBundleIdsByIsbn(string isbn, int pageIndex, int pageSize)
        {
            throw GetNotImplementedException();
        }

        public IEnumerable<string> GetCampaignProducts(DateTime updatedAfter, int pageIndex, int pageSize)
        {
            throw GetNotImplementedException();
        }

        public int GetCampaignProductsCount(DateTime updatedAfter)
        {
            throw GetNotImplementedException();
        }

        public bool HasActiveCampaign(string productId)
        {
            throw GetNotImplementedException();
        }

        public bool IsProductBuyable(Product product)
        {
            return true;
        }

        public bool IsProductBuyable(BundleProduct bundleProduct)
        {
            throw GetNotImplementedException();
        }

        public Dictionary<string, List<WebShop>> GetProductWebshops(List<string> isbns)
        {
            throw GetNotImplementedException();
        }
    }
}