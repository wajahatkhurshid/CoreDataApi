using Gyldendal.Api.CoreData.SolrContracts.Product;
using SolrNet;

namespace Gyldendal.Api.CoreData.GqlToSolrConnector.Model
{
    public class SolrFieldQueryParams
    {
        public SolrFieldQueryParams(ProductSchemaField searchField, double boostFactor, bool isQuotedString, bool isWildCard)
        {
            SearchField = searchField;
            BoostFactor = boostFactor;
            IsWildCard = isWildCard;
            IsQuotedString = isQuotedString;
        }

        public SolrFieldQueryParams()
        {
        }

        public ProductSchemaField SearchField { get; set; }

        public double BoostFactor { get; set; }

        public bool IsWildCard { get; set; }

        public bool IsQuotedString { get; set; }

        public AbstractSolrQuery GetBoostedQuery(string value)
        {
            value = IsQuotedString ? FilterInfo.QuoteString(value) : value;
            return SearchField.ToBoostedFieldQuery(value, BoostFactor, IsWildCard);
        }
    }
}