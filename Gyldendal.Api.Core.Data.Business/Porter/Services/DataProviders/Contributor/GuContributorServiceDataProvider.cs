using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Business.Porter.Services.DataProviders.Contributor
{
    public class GuContributorServiceDataProvider : BaseContributorServiceDataProvider, IContributorServiceDataProvider
    {
        public List<ProfileImage> GetContributorImages(string contributorId, string profileImageUrl)
        {
            return GetContributorProfileImageAsList(profileImageUrl);
        }
    }
}
