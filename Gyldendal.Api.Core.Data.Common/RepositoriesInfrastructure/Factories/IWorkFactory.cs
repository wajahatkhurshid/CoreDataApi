using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using System;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories
{
    public interface IWorkFactory
    {
        List<string> GetDeletedWorks(DataScope dataScope, DateTime? fromLastUpdated);

        GetProductDetailsResponse GetWorkByProductId(DataScope dataScope, ProductType productType, string id);

        GetScopeWorksByProductIdResponse GetScopeWorksByProductId(DataScope scope, string isbn);
    }
}