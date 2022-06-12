using Gyldendal.Api.CoreData.GqlToSolrConnector.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Gyldendal.Api.CoreData.SolrDataProviders.Infrastructure;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Tests
{
    public class BaseFilterTest
    {
        protected readonly IFilterInfoToSolrQueryBuilder SolrQueryBuilder;

        public BaseFilterTest()
        {
            SolrQueryBuilder = new Utils.FilterInfoToSolrQueryBuilder();
        }

        protected void AssertSolrQueryByField(SolrNet.SolrQueryByField solrQueryByField, string fieldName, string fieldValue, [CallerMemberName] string callerName = "")
        {
            Assert.IsNotNull(solrQueryByField, $"Value of {nameof(solrQueryByField)} is null.");

            Assert.AreEqual(solrQueryByField.FieldName, fieldName,
                $"{callerName}: solrQueryByField.FieldName:  Expected value: {fieldName} is not equal to actual value: {solrQueryByField.FieldName}");
            Assert.AreEqual(solrQueryByField.FieldValue, fieldValue,
                $"{callerName}: solrQueryByField.FieldValue:  Expected value: {fieldValue} is not equal to actual value: {solrQueryByField.FieldValue}");
        }

        protected void AssertSolrMultipleCriteriaQuery<T>(SolrNet.SolrMultipleCriteriaQuery solrMultipleCriteriaQuery,
            string fieldName, T[] data, [CallerMemberName] string callerName = "")
        {
            Assert.IsNotNull(solrMultipleCriteriaQuery, $"Value of {nameof(solrMultipleCriteriaQuery)} is null.");

            Assert.AreEqual(solrMultipleCriteriaQuery.Queries.Count(), data.Length);

            var fields = (data.First().GetType().IsEnum
                ? data.Select(d => ((Enum)Convert.ChangeType(d, typeof(Enum))).ToString("D"))
                : data.Select(d => d.ToString())).ToList();

            foreach (var solrQuery in solrMultipleCriteriaQuery.Queries)
            {
                var query = solrQuery as SolrNet.SolrQueryByField;
                Assert.AreEqual(query?.FieldName, fieldName,
                    $"{callerName}: solrQueryByField.FieldName:  Expected value: {fieldName} is not equal to actual value: {query?.FieldName}");

                Assert.IsTrue(fields.Contains(query?.FieldValue.Trim('*')),
                    $"{callerName}: solrQueryByField.FieldValue: {{{string.Join(",", data)}}} does not have expected value: {query?.FieldValue}");
            }
        }

        protected void FieldAssert<T>(FilterInfo filter, string solrProductSchemaField,
            bool excludeFromFacets, bool quoted = true, IEnumerable<FilterInfo> nestedFilters = null, [CallerMemberName] string callerName = "", params T[] data)
        {
            Assert.AreEqual(filter.SolrFieldName, solrProductSchemaField,
                $"filter.SolrFieldName => Expected value: {solrProductSchemaField} is not equal to actual value: {filter.SolrFieldName}");
            Assert.AreEqual(filter.FilterValues.Count(), data.Length);

            var isEnum = data.First().GetType().IsEnum;

            data.ToList().ForEach(d =>
            {
                var value = isEnum ? ((Enum)Convert.ChangeType(d, typeof(Enum))).ToString("D") : d.ToString();
                Assert.IsTrue(filter.FilterValues.Contains(value),
                    $"{callerName}: filter.FilterValues {{{string.Join(",", filter.FilterValues)}}} => FilterValues does not have expected value: {value}");
            });

            Assert.AreEqual(filter.ExcludeFromFacets, excludeFromFacets,
                $"{callerName}: filter.ExcludeFromFacets => Expected value: {excludeFromFacets} is not equal to actual value: {filter.ExcludeFromFacets}");
            Assert.AreEqual(filter.Quoted, quoted,
                $"{callerName}: filter.Quoted => Expected value: {quoted} is not equal to actual value: {filter.Quoted}");
        }
    }
}