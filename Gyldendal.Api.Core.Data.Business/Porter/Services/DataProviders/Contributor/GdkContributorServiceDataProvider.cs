using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders;
using Gyldendal.Api.CoreData.ContentfulProxy.Infrastructure;
using Gyldendal.Api.CoreData.Contracts.Models;
using LoggingManager;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Contributor
{
    public class GdkContributorServiceDataProvider : BaseContributorServiceDataProvider, IContributorServiceDataProvider
    {
        private readonly IContentfulManager _contentfulManager;

        private readonly ILogger _logger;

        public GdkContributorServiceDataProvider(IContentfulManager contentfulManager, ILogger logger)
        {
            _contentfulManager = contentfulManager;
            _logger = logger;
        }
        public List<ProfileImage> GetContributorImages(string contributorId, string profileImageUrl)
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
