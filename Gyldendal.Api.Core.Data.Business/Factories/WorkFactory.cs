using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using System;
using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Business.Factories
{
    public class WorkFactory : CoreDataFactory<IWorkRepository>, IWorkFactory
    {
        /// <summary>
        /// Constructor of work factory
        /// </summary>
        /// <param name="repositories"></param>
        public WorkFactory(IEnumerable<IWorkRepository> repositories) : base(repositories)
        {
        }

        /// <summary>
        /// Returns List of Deleted Work
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="fromLastUpdated"></param>
        /// <returns></returns>
        public List<string> GetDeletedWorks(DataScope dataScope, DateTime? fromLastUpdated)
        {
            return this[dataScope].GetDeletedWorks(fromLastUpdated);
        }

        /// <summary>
        /// Returns work object with product details
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="id"></param>
        /// <param name="productType"></param>
        /// <returns></returns>
        public GetProductDetailsResponse GetWorkByProductId(DataScope dataScope, ProductType productType, string id)
        {
            return this[dataScope].GetWorkByProductId(id, productType);
        }

        public GetScopeWorksByProductIdResponse GetScopeWorksByProductId(DataScope dataScope, string isbn)
        {
            return this[dataScope].GetScopeWorks(isbn);
        }
    }
}