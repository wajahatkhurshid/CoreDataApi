using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Repositories.Global;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.Logging;
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

namespace Gyldendal.Api.CoreData.Business.Repositories.GPlus
{
    public class ContributorRepository : BaseContributorRepository, IContributorRepository
    {
        public ContributorRepository(koncerndata_webshops_Entities kdEntities, IProductDataProvider productDataProvider, IContentfulManager contentfulManager, ILogger logger)
            : base(DataScope.GyldendalPlus, kdEntities, productDataProvider, contentfulManager, logger)
        {
        }

        /// <summary>
        /// Gets details of a contributor
        /// </summary>
        /// <param name="id">Identifier of a contributor</param>
        /// <returns>Details of a contributor</returns>
        public T GetContributor<T>(string id) where T : BaseContributorDetails
        {
            var coreDataContributor = GetCoreDataContributor(id, DataScope.GyldendalPlus, out var webShops);

            if (coreDataContributor == null)
            {
                return null;
            }

            T contributorDetail;

            if (typeof(T) == typeof(ContributorDetails))
            {
                contributorDetail = (T)(object)coreDataContributor.ToContributorDetails();
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

            contributorDetail.WebShopsId = webShops;
            return contributorDetail;
        }

        /// <summary>
        /// Gets all contributors for given type and webshop
        /// </summary>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>List of contributors</returns>
        public SearchContributorResponse<T> GetContributors<T>(SearchContributorRequest searchRequest) where T : BaseContributorDetails
        {
            var kdContributors = GetContributorsFromKd(searchRequest);

            if (typeof(T) == typeof(ContributorDetails))
            {
                var contributors = kdContributors.Select(x => x.ToCoreDataContributor());
                return GetResponse(searchRequest, (IEnumerable<T>)contributors);
            }
            else if (typeof(T) == typeof(ContributorDetailsV2))
            {
                var contributors = kdContributors.Select(x => x.ToCoreDataContributorV2(GetContributorImages(x.contributor_id, x.contributor_foto)));
                return GetResponse(searchRequest, (IEnumerable<T>)contributors);
            }
            else
            {
                throw new ArgumentException($"Invalid argument for generic type: {nameof(T)}");
            }
        }

        public int GetUpdatedContributorsCount(DateTime updateAfterDateTime) => GetContributorUpdateInfoQuery(updateAfterDateTime).Count();

        public IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DateTime updatedAfterDateTime, int pageIndex, int pageSize)
        {
            ValidatePagination(pageIndex, pageSize);

            var skip = pageIndex * pageSize;
            var contributors = GetContributorUpdateInfoQuery(updatedAfterDateTime)
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return contributors;
        }

        private IQueryable<ContributorUpdateInfo> GetContributorUpdateInfoQuery(DateTime updateAfterDateTime)
        {
            var baseQuery = GetContributorUpdateInfoBaseQuery();

            return baseQuery.Where(x => x.UpdateTime > updateAfterDateTime).OrderBy(cui => cui.ContributorId);
        }

        public ContributorUpdateInfo GetContributorUpdateInfo(string contributorId)
        {
            return GetContributorUpdateInfoBaseQuery().FirstOrDefault(x => x.ContributorId == contributorId);
        }

        private IQueryable<ContributorUpdateInfo> GetContributorUpdateInfoBaseQuery()
        {
            return from contLog in KdEntities.DEA_KDWS_GPlus_ConsolidatedContributorsLogView
                let prodContLogAction = contLog.Action.Equals("Deleted", StringComparison.OrdinalIgnoreCase)
                    ? ContributorUpdateType.Deleted
                    : ContributorUpdateType.Updated
                select new ContributorUpdateInfo
                {
                    ContributorId = contLog.ContributorId,
                    UpdateTime = contLog.Timestamp,
                    UpdateType = prodContLogAction
                };
        }

        private IEnumerable<DEA_KDWS_GPlusContributors> GetContributorsFromKd(SearchContributorRequest searchRequest)
        {
            var intContributors = searchRequest.ContributorType.Select(x => (int)x);

            var kdContributors = (KdEntities.DEA_KDWS_GPlusContributors.Join(KdEntities.DEA_KDWS_GPlusproductcontributors,
                    gPlusCont => gPlusCont.contributor_id, gPlusCont => gPlusCont.contributor_id,
                    (gPlusContributor, gPlusProductContributor) => new { gPlusContributor, gPlusProductContributor })
                .Where(x => intContributors.Contains(x.gPlusProductContributor.role_id))
                .OrderBy(x => x.gPlusContributor.contributor_id)
                .Select(x => x.gPlusContributor)
                .Skip(searchRequest.PageIndex * searchRequest.PageSize)
                .Take(searchRequest.PageSize).AsEnumerable()).Distinct();
            return kdContributors;
        }
    }
}