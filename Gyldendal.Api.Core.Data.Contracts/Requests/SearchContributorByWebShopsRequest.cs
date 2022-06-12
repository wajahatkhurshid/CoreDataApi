using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Requests.Common;

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class SearchContributorByWebShopsRequest : SearchContributorRequestBase
    {
        /// <summary>
        /// WebShops array
        /// </summary>
        public WebShop[] WebShops { get; set; }
    }
}
