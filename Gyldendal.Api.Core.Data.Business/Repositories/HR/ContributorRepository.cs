using Gyldendal.Api.CommonContracts;
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

namespace Gyldendal.Api.CoreData.Business.Repositories.HR
{
    public class ContributorRepository : BaseContributorRepository, IContributorRepository
    {
        public ContributorRepository(koncerndata_webshops_Entities kdEntities, IProductDataProvider productDataProvider, IContentfulManager contentfulManager, ILogger logger)
            : base(DataScope.HansReitzelShop, kdEntities, productDataProvider, contentfulManager, logger)
        {
        }

        /// <summary>
        /// Gets details of a contributor
        /// </summary>
        /// <param name="id">Identifier of a contributor</param>
        /// <returns>Details of a contributor</returns>
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
                contributorDetail = (T)(object)coreDataContributor.ToContributorDetails(WebShop.HansReitzel);
            }
            else if (typeof(T) == typeof(ContributorDetailsV2))
            {
                contributorDetail = (T)(object)coreDataContributor
                    .ToContributorDetailsV2(GetContributorProfileImageAsList(coreDataContributor.contributor_foto), WebShop.HansReitzel);
            }
            else
            {
                throw new ArgumentException($"Invalid argument for generic type: {nameof(T)}");
            }

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
                var contributors = kdContributors.Select(x => x.ToCoreDataContributorV2(GetContributorProfileImageAsList(x.contributor_foto)));
                return GetResponse(searchRequest, (IEnumerable<T>)contributors);
            }
            else
            {
                throw new ArgumentException($"Invalid argument for generic type: {nameof(T)}");
            }
        }

        public int GetUpdatedContributorsCount(DateTime updateAfterDateTime)
        {
            var updatedContributors = GetContributorDetails(updateAfterDateTime);

            return updatedContributors.Count();
        }

        public IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DateTime updatedAfterDateTime, int pageIndex, int pageSize)
        {
            ValidatePagination(pageIndex, pageSize);

            var skip = pageIndex * pageSize;
            var updatedContributors = GetContributorDetails(updatedAfterDateTime);
            updatedContributors = updatedContributors.Skip(skip).Take(pageSize);

            return updatedContributors.ToList();
        }

        public ContributorUpdateInfo GetContributorUpdateInfo(string contributorId)
        {
            throw new NotImplementedException();
        }

        private IQueryable<ContributorUpdateInfo> GetContributorDetails(DateTime updateAfterDateTime)
        {
            return (from contLog in KdEntities.DEA_KDWS_HRContributorsLog
                    select new ContributorUpdateInfo
                    {
                        ContributorId = contLog.forfatterID,
                        UpdateTime = contLog.CreatedDate,
                        UpdateType = contLog.Action.Equals("Deleted", StringComparison.OrdinalIgnoreCase)
                            ? ContributorUpdateType.Deleted
                            : ContributorUpdateType.Updated
                    })
                .GroupBy(x => x.ContributorId).Select(x => x.FirstOrDefault(c => c.UpdateTime == x.Max(v => v.UpdateTime)))
                .Where(x => x.UpdateTime > updateAfterDateTime)
                .OrderBy(cui => cui.ContributorId);
        }

        private IEnumerable<DEA_KDWS_HRContributors> GetContributorsFromKd(SearchContributorRequest searchRequest)
        {
            var intContributors = searchRequest.ContributorType.Select(x => (int)x);

            var kdContributors = (KdEntities.DEA_KDWS_HRContributors.Where(x =>
                    x.DEA_KDWS_HRproductcontributors.Any(y => intContributors.Contains(y.role_id)))
                .OrderBy(x => x.contributor_id)
                .Skip(searchRequest.PageIndex * searchRequest.PageSize)
                .Take(searchRequest.PageSize).AsEnumerable()).Distinct();

            return kdContributors;
        }
    }
}