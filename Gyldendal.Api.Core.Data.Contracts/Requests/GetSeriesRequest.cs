using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;

namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class GetSeriesRequest
    {
        public GetSeriesRequestType RequestType { get; set; }

        public string Subject { get; set; }

        public string Area { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public SortBy SortBy { get; set; } = SortBy.Desc;

        public SeriesOrderBy OrderBy { get; set; } = SeriesOrderBy.Name;
    }
}
