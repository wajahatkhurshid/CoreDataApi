using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    public class GetSeriesResponse
    {
        public List<Models.Series> Series { get; set; }

        public int Count { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
