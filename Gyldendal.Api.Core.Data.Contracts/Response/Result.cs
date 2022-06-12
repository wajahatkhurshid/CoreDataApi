using System.Collections.Generic;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Gyldendal.Api.CoreData.Contracts.Response
{
    /// <summary>
    /// Response of Solr Search from CoreDataServices
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public List<T> Results { get; set; }
    }
}
