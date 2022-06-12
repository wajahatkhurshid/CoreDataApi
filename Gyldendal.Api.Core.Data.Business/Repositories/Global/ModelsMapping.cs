using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.Global
{
    public static class ModelsMapping
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static ContributorUpdateInfo ToContributorUpdateInfo(ConsolidatedContributorLogView x)
        {
            return new ContributorUpdateInfo()
            {
                ContributorId = x.contributor_id,
                UpdateTime = x.CreatedDate,
                UpdateType = x.Action.ToContributorUpdateType()
            };
        }

        public static ContributorDetails ToContributorDetails(this ConsolidatedContributorView ccv, koncerndata_webshops_Entities kdEntities)
        {
            return new ContributorDetails()
            {
                AuthorNumber = ccv.contributor_navn,
                Bibliography = ccv.contributor_langBeskrivelse_tekst,
                Biography = ccv.contributor_langbeskrivelse,
                ContributorName = ccv.contributor_navn,
                Photo = ccv.contributor_foto,
                Id = ccv.contributor_id,
                Url = ccv.contributor_profileLink,
                ContributorFirstName = ccv.contributor_fornavn,
                ContributorLastName = ccv.contributor_efternavn,
                WebShopsId =
                    kdEntities.ConsolidatedProductContributorView.Where(x => x.contributor_id == ccv.contributor_id)
                        .Select(x => (WebShop)x.Website_Enum_Id).Distinct().ToList()
            };
        }

        public static ContributorDetailsV2 ToContributorDetailsV2(this ConsolidatedContributorView ccv, koncerndata_webshops_Entities kdEntities, List<ProfileImage> images)
        {
            return new ContributorDetailsV2
            {
                AuthorNumber = ccv.contributor_navn,
                Bibliography = ccv.contributor_langBeskrivelse_tekst,
                Biography = ccv.contributor_langbeskrivelse,
                ContributorName = ccv.contributor_navn,
                Photos = images,
                Id = ccv.contributor_id,
                Url = ccv.contributor_profileLink,
                ContributorFirstName = ccv.contributor_fornavn,
                ContributorLastName = ccv.contributor_efternavn,
                WebShopsId =
                    kdEntities.ConsolidatedProductContributorView.Where(x => x.contributor_id == ccv.contributor_id)
                        .Select(x => (WebShop)x.Website_Enum_Id).Distinct().ToList()
            };
        }

        public static ContributorDetails ToContributorDetails(this ConsolidatedContributorView ccv, params WebShop[] webShops)
        {
            return new ContributorDetails()
            {
                AuthorNumber = ccv.contributor_navn,
                Bibliography = ccv.contributor_langBeskrivelse_tekst,
                Biography = ccv.contributor_langbeskrivelse,
                ContributorName = ccv.contributor_navn,
                Photo = ccv.contributor_foto,
                Id = ccv.contributor_id,
                Url = ccv.contributor_profileLink,
                ContributorFirstName = ccv.contributor_fornavn,
                ContributorLastName = ccv.contributor_efternavn,
                WebShopsId = webShops?.ToList()
            };
        }

        public static ContributorDetailsV2 ToContributorDetailsV2(this ConsolidatedContributorView ccv, List<ProfileImage> photos, params WebShop[] webShops)
        {
            return new ContributorDetailsV2()
            {
                AuthorNumber = ccv.contributor_navn,
                Bibliography = ccv.contributor_langBeskrivelse_tekst,
                Biography = ccv.contributor_langbeskrivelse,
                ContributorName = ccv.contributor_navn,
                Photos = photos,
                Id = ccv.contributor_id,
                Url = ccv.contributor_profileLink,
                ContributorFirstName = ccv.contributor_fornavn,
                ContributorLastName = ccv.contributor_efternavn,
                WebShopsId = webShops?.ToList()
            };
        }
    }
}