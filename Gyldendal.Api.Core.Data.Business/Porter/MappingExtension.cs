using Gyldendal.Api.CoreData.Common;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Common.WebUtils.Exceptions;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using ProductType = Gyldendal.Api.CoreData.Contracts.Enumerations.ProductType;
using SeriesOrderBy = Gyldendal.Api.CoreData.Contracts.Enumerations.SeriesOrderBy;
using SortBy = Gyldendal.Api.CommonContracts.SortBy;
using WebShop = Gyldendal.Api.CommonContracts.WebShop;

namespace Gyldendal.Api.CoreData.Business.Porter
{
    public static class MappingExtension
    {
        public static PorterApi.WebShop ToPorterWebShop(this WebShop sourceEnum)
        {
            switch (sourceEnum)
            {
                case WebShop.None:
                    return PorterApi.WebShop.None;
                case WebShop.Gu:
                    return PorterApi.WebShop.GU;
                case WebShop.HansReitzel:
                    return PorterApi.WebShop.HansReitzel;
                case WebShop.MunksGaard:
                    return PorterApi.WebShop.MunksGaard;
                case WebShop.TradeGyldendalDk:
                    return PorterApi.WebShop.GyldendalPlus;
                case WebShop.ClubBogklub:
                case WebShop.ClubBoerne:
                case WebShop.ClubSamleren:
                case WebShop.ClubKrimi:
                case WebShop.ClubPsykeSjael:
                case WebShop.ClubHistorie:
                case WebShop.ClubPaedagogisk:
                case WebShop.ClubBoerne3To5:
                case WebShop.ClubBoerne5To10:
                case WebShop.ClubFlamingo:
                case WebShop.ClubSundtLiv:
                    return PorterApi.WebShop.GyldendalPlus;
                default:
                    throw new ProcessException((ulong)ErrorCodes.InvalidWebSite, ErrorCodes.InvalidWebSite.GetDescription(), Extensions.CoreDataSystemName);

            }
        }
        public static WebShop ToCoreDataWebShop(this PorterApi.WebShop sourceEnum)
        {
            switch (sourceEnum)
            {
                case PorterApi.WebShop.None:
                    return WebShop.None;
                case PorterApi.WebShop.GU:
                    return WebShop.Gu;
                case PorterApi.WebShop.HansReitzel:
                    return WebShop.HansReitzel;
                case PorterApi.WebShop.MunksGaard:
                    return WebShop.MunksGaard;
                case PorterApi.WebShop.Pressesite:
                    return WebShop.TradeGyldendalDk;
                case PorterApi.WebShop.GyldendalPlus:
                    return WebShop.ClubBogklub;
                case PorterApi.WebShop.Harmoney:
                    return WebShop.ClubBogklub;
                default:
                    throw new ProcessException((ulong)ErrorCodes.InvalidWebSite, ErrorCodes.InvalidWebSite.GetDescription(), Extensions.CoreDataSystemName);

            }
        }
        public static PorterApi.ProductType ToPorterProductType(this ProductType sourceEnum)
        {
            switch (sourceEnum)
            {
                case ProductType.Bundle:
                    return PorterApi.ProductType.Bundle;
                case ProductType.SingleProduct:
                    return PorterApi.ProductType.SingleProduct;
                default:
                    return PorterApi.ProductType.None;
            }
        }
        public static ProductType ToCoreDataProductType(this PorterApi.ProductType sourceEnum)
        {
            switch (sourceEnum)
            {
                case PorterApi.ProductType.Bundle:
                    return ProductType.Bundle;
                case PorterApi.ProductType.SingleProduct:
                    return ProductType.SingleProduct;
                default:
                    return ProductType.None;
            }
        }
        public static PorterApi.SeriesType ToPorterSeriesType(this GetSeriesRequestType sourceEnum)
        {
            switch (sourceEnum)
            {
                case GetSeriesRequestType.SeriesOnly:
                    return PorterApi.SeriesType.Series;
                case GetSeriesRequestType.SystemSeriesOnly:
                    return PorterApi.SeriesType.SystemSeries;
                default:
                    return PorterApi.SeriesType.All;
            }
        }
        public static GetSeriesRequestType ToCoreDataSeriesType(this PorterApi.SeriesType sourceEnum)
        {
            switch (sourceEnum)
            {
                case PorterApi.SeriesType.Series:
                    return GetSeriesRequestType.SeriesOnly;
                case PorterApi.SeriesType.SystemSeries:
                    return GetSeriesRequestType.SystemSeriesOnly;
                default:
                    return GetSeriesRequestType.All;
            }
        }
        public static PorterApi.SeriesOrderBy ToPorterSeriesOrderBy(this SeriesOrderBy sourceEnum)
        {
            switch (sourceEnum)
            {
                case SeriesOrderBy.Name:
                    return PorterApi.SeriesOrderBy.Name;
                default:
                    return PorterApi.SeriesOrderBy.UpdatedTimestamp;
            }
        }
        public static SeriesOrderBy ToCoreDataOrderBy(this PorterApi.SeriesOrderBy sourceEnum)
        {
            switch (sourceEnum)
            {
                case PorterApi.SeriesOrderBy.Name:
                    return SeriesOrderBy.Name;
                default:
                    return SeriesOrderBy.LastUpdated;
            }
        }
        public static PorterApi.SortBy ToPorterSortBy(this SortBy sourceEnum)
        {
            switch (sourceEnum)
            {
                case SortBy.Asc:
                    return PorterApi.SortBy.Asc;
                default:
                    return PorterApi.SortBy.Desc;
            }
        }
        public static SortBy ToCoreDataSortBy(this PorterApi.SortBy sourceEnum)
        {
            switch (sourceEnum)
            {
                case PorterApi.SortBy.Asc:
                    return SortBy.Asc;
                default:
                    return SortBy.Desc;
            }
        }
    }
}