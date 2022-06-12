using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class WorkSearchRequest
    {
        public WebShop Webshop { get; set; }

        public string Gql { get; set; } = "";

        public FilterInfo Filters { get; set; }

        public PagingInfo Paging { get; set; }

        public OrderBy OrderBy { get; set; }

        public SortBy SortBy { get; set; } = SortBy.Desc;
    }
}