using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Repositories.RosinanteCo
{
    public class ContributorRepository: BaseRepository, IContributorRepository
    {
        public ContributorRepository(koncerndata_webshops_Entities kdEntities) : 
            base(DataScope.GyldendalDkShop, kdEntities)
        {
        }

        public ContributorDetails GetContributor(string id)
        {
            var kdContributor = KdEntities.DEA_KDWS_ROSCOContributors.FirstOrDefault(x => x.contributor_id == id);

            return kdContributor?.ToCoreDataContributor();
        }

        public SearchContributorResponse GetContributors(SearchContributorRequest searchRequest)
        {
            var intContributors = searchRequest.ContributorType.Select(x => (int)x);

            var kdContributors = (KdEntities.DEA_KDWS_ROSCOContributors.Join(KdEntities.DEA_KDWS_ROSCOproductcontributors, 
                    rosCoCont => rosCoCont.contributor_id, gdkPCont => gdkPCont.contributor_id, (rosCoContributor, gdkProductContributor) => new { rosCoContributor, gdkProductContributor })
                .Where(x => intContributors.Contains(x.gdkProductContributor.role_id))
                .OrderBy(x => x.rosCoContributor.contributor_id)
                .Select(x => x.rosCoContributor)
                .Skip(searchRequest.PageIndex * searchRequest.PageSize)
                .Take(searchRequest.PageSize).AsEnumerable()).Distinct();
            
            var contributors = kdContributors.Select<DEA_KDWS_ROSCOContributors, ContributorDetails>(x => x.ToCoreDataContributor()).ToList();

            var response = new SearchContributorResponse
            {
                PageSize = searchRequest.PageSize,
                PageIndex = searchRequest.PageIndex,
                Contributors = contributors,
                TotalRecords = contributors.Count()
            };
            return response;
        }

        public int GetUpdatedContributorsCount(DateTime updateAfterDateTime)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContributorUpdateInfo> GetContributorsUpdateInfo(DateTime updatedAfterDateTime, int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
    }
}
