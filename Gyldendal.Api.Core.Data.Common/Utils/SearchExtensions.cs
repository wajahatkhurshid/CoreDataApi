using Gyldendal.Api.CoreData.Common.Request;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Gyldendal.Api.CoreData.Contracts.Models;
using NewRelic.Api.Agent;

namespace Gyldendal.Api.CoreData.Common.Utils
{
    public static class SearchExtensions
    {
        private const int DefaultPageSize = 20;

        public static WorkProductSearchRequest ToWorkProductSearchRequest(this WorkSearchRequest request)
        {
            var searchRequest = new WorkProductSearchRequest
            {
                Gql = request.Gql,
                OrderBy = request.OrderBy,
                Paging = request.Paging ?? new PagingInfo { PageIndex = 0, PageSize = DefaultPageSize },
                SortBy = request.SortBy,
                WebShops = new[] { request.Webshop },
                CallingWebShop = request.Webshop,
                Filters = ToFiltersDictionary(request.Filters),
                FacetTypes = GetV1FacetTypes()
            };
            return searchRequest;
        }

        [Trace]
        public static WorkProductSearchRequest ToWorkProductSearchRequest(this ProductSearchRequest request)
        {
            var searchRequest = new WorkProductSearchRequest
            {
                Gql = request.Gql,
                OrderBy = request.OrderBy,
                Paging = request.Paging ?? new PagingInfo { PageIndex = 0, PageSize = DefaultPageSize },
                SortBy = request.SortBy,
                WebShops = new[] { request.Webshop },
                CallingWebShop = request.Webshop,
                Filters = ToFiltersDictionary(request.Filters),
                FacetTypes = GetV1FacetTypes()
            };
            return searchRequest;
        }

        [Trace]
        public static WorkProductSearchRequest ToWorkProductSearchRequest(this ProductSearchRequestEx request)
        {
            var searchRequest = new WorkProductSearchRequest
            {
                Gql = request.Gql,
                OrderBy = request.OrderBy,
                Paging = request.Paging ?? new PagingInfo { PageIndex = 0, PageSize = DefaultPageSize },
                SortBy = request.SortBy,
                CallingWebShop = request.CallingWebShop,
                Filters = ToFiltersDictionary(request.Filters),
                FacetTypes = GetV1FacetTypes()
            };
            return searchRequest;
        }

        public static WorkProductSearchRequest ToWorkProductSearchRequest(this WorkSearchRequestV2 request)
        {
            var searchRequest = new WorkProductSearchRequest
            {
                Gql = request.Gql,
                OrderBy = request.OrderBy,
                Paging = request.Paging ?? new PagingInfo { PageIndex = 0, PageSize = DefaultPageSize },
                SortBy = request.SortBy,
                WebShops = new[] { request.Webshop },
                CallingWebShop = request.Webshop,
                Filters = request.Filters?.ToWorkProductSearchFilterDictionary() ??
                          new Dictionary<WorkProductSearchFilter, List<string>>(),
                FacetTypes = request.FacetTypes ?? new List<FacetType>(),
                PriceRangeFacetParams = request.PriceRangeFacetParams,
                PriceRangeFilters = request.PriceRangeFilters,
                UseGqlExpressionTree = true
            };
            return searchRequest;
        }

        [Trace]
        public static WorkProductSearchRequest ToWorkProductSearchRequest(this ProductSearchRequestV2 request)
        {
            var searchRequest = new WorkProductSearchRequest
            {
                Gql = request.Gql,
                OrderBy = request.OrderBy,
                Paging = request.Paging ?? new PagingInfo { PageIndex = 0, PageSize = DefaultPageSize },
                SortBy = request.SortBy,
                WebShops = new[] { request.Webshop },
                CallingWebShop = request.Webshop,
                Filters = request.Filters?.ToWorkProductSearchFilterDictionary() ??
                          new Dictionary<WorkProductSearchFilter, List<string>>(),
                FacetTypes = request.FacetTypes ?? new List<FacetType>(),
                PriceRangeFacetParams = request.PriceRangeFacetParams,
                PriceRangeFilters = request.PriceRangeFilters,
                UseGqlExpressionTree = true
            };
            return searchRequest;
        }

        [Trace]
        public static WorkProductSearchRequest ToWorkProductSearchRequest(this GetProductsByGqlRequest request)
        {
            var searchRequest = new WorkProductSearchRequest
            {
                Gql = request.Gql,
                FacetTypes = new List<FacetType>(),
                Filters = new Dictionary<WorkProductSearchFilter, List<string>>(),
                Paging = request.Paging ?? new PagingInfo { PageIndex = 0, PageSize = DefaultPageSize },
                UseGqlExpressionTree = true
            };
            return searchRequest;
        }

        public static WorkProductSearchRequest ToWorkProductSearchRequest(this WorkSearchRequestV3 request)
        {
            var searchRequest = new WorkProductSearchRequest
            {
                Gql = request.Gql,
                OrderBy = request.OrderBy,
                Paging = request.Paging ?? new PagingInfo { PageIndex = 0, PageSize = DefaultPageSize },
                SortBy = request.SortBy,
                WebShops = request.PrimaryWebShops,
                SecondaryWebShops = request.SecondaryWebShops,
                CallingWebShop = request.CallingWebShop,
                Filters = request.Filters?.ToWorkProductSearchFilterDictionary() ??
                          new Dictionary<WorkProductSearchFilter, List<string>>(),
                FacetTypes = request.FacetTypes ?? new List<FacetType>(),
                PriceRangeFacetParams = request.PriceRangeFacetParams,
                PriceRangeFilters = request.PriceRangeFilters,
                UseGqlExpressionTree = true
            };
            return searchRequest;
        }

        public static List<KeyValuePair<PriceRange, int>> ToPriceRangeFacet(
            this ICollection<KeyValuePair<string, int>> priceFacets)
        {
            if (priceFacets == null || !priceFacets.Any()) return null;
            var pattern = new Regex(@"(\d+(\.\d+)?)");

            return (from priceRange in priceFacets
                    let patternMatches = pattern.Matches(priceRange.Key)
                    let pricePatternsFound = patternMatches.Count > 1
                    let range = new PriceRange
                    {
                        From = pricePatternsFound
                            ? decimal.Parse(patternMatches[0].Value, CultureInfo.InvariantCulture)
                            : 0,
                        To = pricePatternsFound ? decimal.Parse(patternMatches[1].Value, CultureInfo.InvariantCulture) : 0
                    }
                    select new KeyValuePair<PriceRange, int>(range, priceRange.Value)).ToList();
        }

        public static WorkProductSearchRequest Clone(this WorkProductSearchRequest request)
        {
            return new WorkProductSearchRequest
            {
                Gql = request.Gql,
                OrderBy = request.OrderBy,
                Paging = new PagingInfo
                {
                    PageIndex = request.Paging.PageIndex,
                    PageSize = request.Paging.PageSize
                },
                SortBy = request.SortBy,
                WebShops = request.WebShops,
                SecondaryWebShops = request.SecondaryWebShops,
                CallingWebShop = request.CallingWebShop,
                Filters = request.Filters,
                FacetTypes = request.FacetTypes,
                PriceRangeFacetParams = request.PriceRangeFacetParams,
                PriceRangeFilters = request.PriceRangeFilters,
                UseGqlExpressionTree = request.UseGqlExpressionTree
            };
        }

        public static void AddFilter(this Dictionary<WorkProductSearchFilter, List<string>> filters,
            WorkProductSearchFilter workProductSearchFilter,
            IEnumerable<string> data)
        {
            if (filters.ContainsKey(workProductSearchFilter))
            {
                filters[workProductSearchFilter].AddRange(data);
            }
            else
            {
                filters.Add(workProductSearchFilter, data.ToList());
            }
        }

        private static List<FacetType> GetV1FacetTypes()
        {
            return new List<FacetType>
            {
                FacetType.SystemName,
                FacetType.SeriesName,
                FacetType.MaterialTypeName,
                FacetType.MediaTypeName,
                FacetType.LevelName,
                FacetType.SubjectWithAreaAndSubarea,
                FacetType.Subjects,
                FacetType.Areas,
                FacetType.SubAreas,
                FacetType.HasTrialAccess
            };
        }

        [Trace]
        private static Dictionary<WorkProductSearchFilter, List<string>> ToWorkProductSearchFilterDictionary(
            this Dictionary<FilterType, List<string>> filters)
        {
            return filters.ToDictionary(
                x => (WorkProductSearchFilter)Enum.Parse(typeof(WorkProductSearchFilter), $"{x.Key:G}"), y => y.Value);
        }

        private static Dictionary<WorkProductSearchFilter, List<string>> ToFiltersDictionary(FilterInfo filters)
        {
            if (filters == null)
            {
                return new Dictionary<WorkProductSearchFilter, List<string>>();
            }

            return new Dictionary<WorkProductSearchFilter, List<string>>
            {
                {
                    WorkProductSearchFilter.Areas,
                    filters.AreasFilter
                },
                {
                    WorkProductSearchFilter.SystemName,
                    filters.SystemNameFilter
                },
                {
                    WorkProductSearchFilter.LevelName,
                    filters.LevelNameFilter
                },

                {
                    WorkProductSearchFilter.SeriesName,
                    filters.SeriesNameFilter
                },

                {
                    WorkProductSearchFilter.MaterialTypeName,
                    filters.MaterialTypeNameFilter
                },

                {
                    WorkProductSearchFilter.MediaTypeName,
                    filters.MediaTypeNameFilter
                },

                {
                    WorkProductSearchFilter.Subjects,
                    filters.SubjectsFilter
                },

                {
                    WorkProductSearchFilter.SubAreas,
                    filters.SubAreasFilter
                },
            };
        }
    }
}