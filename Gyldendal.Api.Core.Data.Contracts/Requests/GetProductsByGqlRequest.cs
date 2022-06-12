namespace Gyldendal.Api.CoreData.Contracts.Requests
{
    public class GetProductsByGqlRequest
    {
        /// <summary>
        /// GQL to search products in Solr
        /// </summary>
        public string Gql { get; set; }

        /// <summary>
        /// The Paging information
        /// </summary>
        public PagingInfo Paging { get; set; }
    }
}