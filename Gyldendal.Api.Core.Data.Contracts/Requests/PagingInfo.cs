// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class PagingInfo
    {
        public PagingInfo()
        {
            PageSize = 50;
            PageIndex = 0;
        }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
