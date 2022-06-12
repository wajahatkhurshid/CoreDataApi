using System;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders
{
    public interface IProductServiceDataProvider
    {
        Contracts.Models.Product ProvideDataFor_GetProductByIsbn(Contracts.Models.Product product);
        IEnumerable<string> ProvideDataFor_GetModifiedSingleProductCampaignsIsbn(DateTime updatedAfterTicks, string shopName);
        IEnumerable<string> ProvideDataFor_GetModifiedBundleCampaignsPakkeId(DateTime updatedAfterTicks, string shopName);
        IEnumerable<string> ProvideDataFor_GetModifiedBundleCampaignsUpdateInfo(DateTime updatedAfterTicks, string shopName, int pageSize, int pageInfo);
    }
}
