using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Business.Porter.Interfaces.DataProviders
{
    public interface IContributorServiceDataProvider
    {
        List<ProfileImage> GetContributorImages(string contributorId, string profileImageUrl);
    }
}
