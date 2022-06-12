using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface IWorkRepository : ICoreDataRepository
    {
        List<string> GetDeletedWorks(DateTime? fromDate);

        GetProductDetailsResponse GetWorkByProductId(string id, ProductType productType);

        GetScopeWorksByProductIdResponse GetScopeWorks(string isbn);
    }
}