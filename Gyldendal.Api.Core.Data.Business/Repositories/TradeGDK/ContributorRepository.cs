using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.InternalObjects;
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

namespace Gyldendal.Api.CoreData.Business.Repositories.TradeGDK
{
    public class ContributorRepository : BaseContributorRepository, IContributorRepository
    {
        public ContributorRepository(koncerndata_webshops_Entities kdEntities, IProductDataProvider productDataProvider, IContentfulManager contentfulManager, ILogger logger) :
            base(DataScope.TradeGyldendalDk, kdEntities, productDataProvider, contentfulManager, logger)
        {
        }

        public T GetContributor<T>(string id) where T : BaseContributorDetails
        {
            var coreDataContributor = GetCoreDataContributor(id, DataScope.TradeGyldendalDk, out var webShops);

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
                .Take(pageSize).ToList().Select(c => new ContributorUpdateInfo
                {
                    ContributorId = ModelsMapping.ExtractGdkContributorId(c.ContributorId),
                    UpdateTime = c.UpdateTime,
                    UpdateType = c.UpdateType
                });

            return contributors;
        }

        private IQueryable<ContributorWithDescription> GetContributorUpdateInfoQuery(DateTime updateAfterDateTime)
        {
            var baseQuery = GetContributorUpdateInfoBaseQuery();

            return baseQuery.Where(x => x.UpdateTime > updateAfterDateTime).OrderBy(cui => cui.ContributorId);
        }

        public ContributorUpdateInfo GetContributorUpdateInfo(string contributorId)
        {
            contributorId = contributorId.Replace("F", "0-").Replace("S", "1-");
            var contributorUpdateInfo = GetContributorUpdateInfoBaseQuery().FirstOrDefault(x => x.ContributorId == contributorId);
            if (contributorUpdateInfo == null)
                return null;
            return new ContributorUpdateInfo
            {
                ContributorId = ModelsMapping.ExtractGdkContributorId(contributorUpdateInfo.ContributorId),
                UpdateTime = contributorUpdateInfo.UpdateTime,
                UpdateType = contributorUpdateInfo.UpdateType
            };
        }

        private IQueryable<ContributorWithDescription> GetContributorUpdateInfoBaseQuery()
        {
            return from contLog in KdEntities.DEA_KDWS_GDK_ConsolidatedContributorsLogView
                join cont in KdEntities.DEA_KDWS_GDKContributors on contLog.ContributorId equals cont.contributor_id
                let prodContLogAction = contLog.Action.Equals("Deleted", StringComparison.OrdinalIgnoreCase)
                    ? ContributorUpdateType.Deleted
                    : ContributorUpdateType.Updated
                select new ContributorWithDescription
                {
                    Description = cont.contributor_langbeskrivelse,
                    ContributorId = contLog.ContributorId,
                    UpdateTime = contLog.Timestamp,
                    UpdateType = prodContLogAction
                };
        }

        private IEnumerable<DEA_KDWS_GDKContributors> GetContributorsFromKd(SearchContributorRequest searchRequest)
        {
            var intContributors = searchRequest.ContributorType.Select(x => (int)x);

            var kdContributors = (KdEntities.DEA_KDWS_GDKContributors.Join(KdEntities.DEA_KDWS_GDKproductcontributors,
                    gdkCont => gdkCont.contributor_id, gdkPCont => gdkPCont.contributor_id,
                    (gdkContributor, gdkProductContributor) => new { gdkContributor, gdkProductContributor })
                .Where(x => intContributors.Contains(x.gdkProductContributor.role_id))
                .OrderBy(x => x.gdkContributor.contributor_id)
                .Select(x => x.gdkContributor)
                .Skip(searchRequest.PageIndex * searchRequest.PageSize)
                .Take(searchRequest.PageSize).AsEnumerable()).Distinct();

            return kdContributors;
        }
    }
}