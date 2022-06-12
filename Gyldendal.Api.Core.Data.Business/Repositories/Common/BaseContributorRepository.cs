using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Common.Logging;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public class BaseContributorRepository : BaseRepository
    {
        private readonly IProductDataProvider _productDataProvider;

        private readonly IContentfulManager _contentfulManager;

        private readonly ILogger _logger;

        protected BaseContributorRepository(DataScope dataScope, koncerndata_webshops_Entities kdEntities, IProductDataProvider productDataProvider, IContentfulManager contentfulManager, ILogger logger) : base(dataScope, kdEntities)
        {
            _productDataProvider = productDataProvider;
            _contentfulManager = contentfulManager;
            _logger = logger;
        }

        protected ConsolidatedContributorView GetCoreDataContributor(string contributorId)
        {
            var kdContributorTask = Task.Run(() => KdEntities.ConsolidatedContributorView.FirstOrDefault(x => x.contributor_id == contributorId));
            kdContributorTask.Wait();

            return kdContributorTask.Result;
        }

        protected List<ProfileImage> GetContributorProfileImageAsList(string profileImageUrl)
        {
            var images = new List<ProfileImage>
            {
                new ProfileImage{Type = "Profile Image", Url = profileImageUrl}
            };

            return images;
        }

        protected SearchContributorResponse<T> GetResponse<T>(SearchContributorRequest searchRequest, IEnumerable<T> contributors) where T : BaseContributorDetails
        {
            var resContributors = contributors.ToList();
            var response = new SearchContributorResponse<T>
            {
                PageIndex = searchRequest.PageIndex,
                PageSize = searchRequest.PageSize,
                Contributors = resContributors,
                TotalRecords = resContributors.Count()
            };

            return response;
        }

        public ConsolidatedContributorView GetCoreDataContributor(string id, DataScope dataScope, out List<WebShop> webShops)
        {
            webShops = null;
            var coreDataContributor = GetCoreDataContributor(id);

            if (coreDataContributor == null)
            {
                return null;
            }

            var webShopsTask = Task.Run(() => _productDataProvider.GetWebsiteIdsByContributorId(id, dataScope));
            webShopsTask.Wait();
            webShops = webShopsTask.Result;

            return coreDataContributor;
        }

        protected List<ProfileImage> GetContributorImages(string contributorId, string profileImageUrl)
        {
            var images = GetContributorProfileImageAsList(profileImageUrl);

            try
            {
                var heroImageUrlTask = Task.Run(() => _contentfulManager.GetContributorHeroImage(contributorId));
                heroImageUrlTask.Wait();

                if (!string.IsNullOrWhiteSpace(heroImageUrlTask.Result))
                {
                    images.Add(new ProfileImage { Type = "Hero Image", Url = heroImageUrlTask.Result });
                }
            }
            catch (Exception e)
            {
                _logger.Error($"AuthorHeroImageError: Error while getting author hero image url from contentful.", e, isGdprSafe: true);
            }

            return images;
        }
    }
}