using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Common.Logging;

namespace Gyldendal.Api.CoreData.Business.Repositories.Global
{
    public class ContributorRepository : BaseContributorRepository, IContributorRepository
    {
        public ContributorRepository(koncerndata_webshops_Entities kdEntities, IProductDataProvider productDataProvider, IContentfulManager contentfulManager, ILogger logger)
            : base(DataScope.Global, kdEntities, productDataProvider, contentfulManager, logger)
        {
        }

        /// <summary>
        /// Returns the contributor detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetContributor<T>(string id) where T : BaseContributorDetails
        {
            var coreDataContributor = KdEntities.ConsolidatedContributorView.FirstOrDefault(x => x.contributor_id == id);

            if (coreDataContributor == null)
            {
                return null;
            }

            T contributorDetail;

            if (typeof(T) == typeof(ContributorDetails))
            {
                contributorDetail = (T)(object)coreDataContributor.ToContributorDetails(KdEntities);
            }
            else if (typeof(T) == typeof(ContributorDetailsV2))
            {
                contributorDetail = (T)(object)coreDataContributor
                    .ToContributorDetailsV2(GetContributorImages(id, coreDataContributor.contributor_foto));
            }
            else
            {
                throw new ArgumentException($"Invalid argument for generic type: {nameof(T)}");
            }

            return contributorDetail;
        }

        public SearchContributorResponse<T> GetContributors<T>(SearchContributorRequest searchRequest) where T : BaseContributorDetails
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        ///  Returns the number of contributors updated after the given DateTime value.
        /// </summary>
        /// <param name="updateAfterDateTime"></param>
        /// <returns>updated contributors</returns>
        public int GetUpdatedContributorsCount(DateTime updateAfterDateTime)
        {
            return KdEntities.ConsolidatedContributorLogView.Count(x => x.CreatedDate > updateAfterDateTime);
        }

        /// <summary>
        /// Returns the asked page of ContributorupdatedInfo objects, for each contributor,
        /// that was updated after the given DateTime, in KoncernDataWebShops database.
        /// </summary>
        /// <param name="updatedAfterDateTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DateTime updatedAfterDateTime, int pageIndex, int pageSize)
        {
            ValidatePagination(pageIndex, pageSize);

            var skip = pageIndex * pageSize;
            return KdEntities.ConsolidatedContributorLogView.Where(x => x.CreatedDate > updatedAfterDateTime)
                .OrderBy(x => x.contributor_id)
                .Skip(skip)
                .Take(pageSize)
                .ToArray()
                .Select(ModelsMapping.ToContributorUpdateInfo)
                .ToArray();
        }

        public ContributorUpdateInfo GetContributorUpdateInfo(string contributorId)
        {
            throw new NotImplementedException();
        }
    }
}