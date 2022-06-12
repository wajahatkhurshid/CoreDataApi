using Gyldendal.Api.CoreData.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Contributor
{
    public class BaseContributorServiceDataProvider
    {
        protected List<ProfileImage> GetContributorProfileImageAsList(string profileImageUrl)
        {
            var images = new List<ProfileImage>
            {
                new ProfileImage{Type = "Profile Image", Url = profileImageUrl}
            };

            return images;
        }
    }
}
