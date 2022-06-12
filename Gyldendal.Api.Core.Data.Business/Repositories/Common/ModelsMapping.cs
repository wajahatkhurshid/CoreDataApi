using System;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.InternalObjects;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Contracts.Models.ProductExtraData;
using Gyldendal.PulsenServices.Api.Contracts;
using Gyldendal.PulsenServices.Api.Contracts.Common;
using Gyldendal.PulsenServices.Api.Contracts.Product;
using Newtonsoft.Json;
using Product = Gyldendal.Api.CoreData.Contracts.Models.Product;
using ProductPrice = Gyldendal.Api.CoreData.Contracts.Models.ProductPrice;
using PulsenProduct = Gyldendal.PulsenServices.Api.Contracts.Product.Product;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public static class ModelsMapping
    {
        private const string MediaproviderUrlVersionPart = "/v1/MediaProvider/GetDigitalMaterial";

        /// <summary>
        /// Generates and returns the Image Url for a System or Series, using a predefined format of baseUrl + Id + ImgExt.
        /// Uses config manager to get the default values of base Url and image extension.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="configManager"></param>
        /// <param name="imageUploadedInRap"></param>
        /// <returns></returns>
        public static string GetSystemSeriesImageUrl(int id, IConfigurationManager configManager, bool imageUploadedInRap)
        {
            if (!(imageUploadedInRap))
            {
                return string.Empty;
            }

            return $"{configManager.SystemSerieImageBaseUrl}{id}{configManager.SystemSerieImageExt}";
        }

        /// <summary>
        /// Get product's Urls based upon their attributes
        /// </summary>
        /// <returns>Product's URLs, , in case of digital products their secured material link is returned</returns>
        /// <remarks>
        /// VSTS Bugs which are fixed.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Bug ID</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>10888</term>
        ///         <description>Proposed solution point 2. Implement same logic to populate value of product URL of Product Methods same as of My Digital Products method</description>
        ///     </item>
        ///     <item>
        ///         <term>14607</term>
        ///         <description>Don't create digitial product link it digital product doesn't have any attachement</description>
        ///     </item>
        /// </list>
        /// </remarks>
        internal static List<ProductUrl> GetProductUrls(ProductUrlInput input)
        {
            var returnUrls = new List<ProductUrl>();

            var productUrl = input.isPhysical ? input.url : (input.hasAttachments ? GetDigitalProductUrl(input.webShop, input.productId, input.mediaType, input.url, input.configManager) : input.url);

            if (string.IsNullOrWhiteSpace(productUrl))
                return returnUrls;

            returnUrls.Add(new ProductUrl
            {
                Url = productUrl,
                UrlType = input.isPhysical ? ProductUrlType.RelatedWebsiteUrl : ProductUrlType.ProductUrl
            });

            return returnUrls;
        }

        /// <summary>
        /// Digital product's URL, creates url based upon parameters whether it should be web address or media provider URL
        /// </summary>
        /// <param name="webshop">Webshop for which Url has to be created for</param>
        /// <param name="productId">Id of the product whose product url has to be constructed</param>
        /// <param name="mediaType">Media Type of the website</param>
        /// <param name="webAddress">Webaddress of the product from uderlying provider</param>
        /// <param name="configManager"></param>
        /// <returns>Digital Product's URL</returns>
        /// <remarks>
        /// Bug id 10888 proposed solution point 2
        /// Implement same logic to populate value of product URL of Product Methods same as of My Digital Products method
        /// </remarks>
        public static string GetDigitalProductUrl(WebShop webshop, string productId, string mediaType, string webAddress, IConfigurationManager configManager)
        {
            var eBookMediaTypes = configManager.DownloadableMaterialMediaTypes;

            var mediaProviderBaseUrl = $"{configManager.MediaProviderUrl}{MediaproviderUrlVersionPart}";

            return eBookMediaTypes.Contains(mediaType?.Trim() ?? string.Empty)
                ? $"{mediaProviderBaseUrl}/{productId}/{(int)webshop}"
                : webAddress;
        }

        public static Work GetClubWorkForTrade(Work baseWork, PulsenProduct pulsenProdDetail, Clubs clubId)
        {
            var clonedWork = baseWork.Clone();

            var clonedWorkProduct = clonedWork.Products.First();

            clonedWorkProduct.WebShop = clonedWork.WebShop = clubId.GetGyldendalWebShop();

            // Get labels list.
            clonedWorkProduct.Labels = pulsenProdDetail.Labels?.Where(x => x.Key == clubId).Select(x => x.Value).FirstOrDefault();

            SetProductPricesForTrade(pulsenProdDetail, clubId, clonedWorkProduct);

            if (clubId != Clubs.GyldendalDk)
            {
                clonedWorkProduct.SeoText = pulsenProdDetail.ShortDescription;
            }
            // Get product description
            clonedWorkProduct.Description = pulsenProdDetail.Description;

            // ReSharper disable once PossibleNullReferenceException
            clonedWorkProduct.Id = pulsenProdDetail.ArticleId;

            clonedWork.Areas = GetWorkAreasForTrade(pulsenProdDetail);

            var extraData = new GyldendalPlusExtraData { Intro = pulsenProdDetail.Intro };
            clonedWorkProduct.ExtraData = JsonConvert.SerializeObject(extraData);

            clonedWorkProduct.UpdateDueOn = pulsenProdDetail.UpdateDueOn;
            return clonedWork;
        }

        private static void SetProductPricesForTrade(PulsenProduct pulsenProdDetail, Clubs clubId, Product clonedWorkProduct)
        {
            var clubPricing = pulsenProdDetail.PricingDetail?.FirstOrDefault(x => x.Key == clubId).Value;
            var defaultPrice = clubPricing?.DefaultPrice;

            clonedWorkProduct.DefaultPrice = defaultPrice?.PriceWithVat ?? 0;
            clonedWorkProduct.DiscountPercentage = defaultPrice?.DiscountPercentage ?? 0;
            clonedWorkProduct.DiscountedPrice = defaultPrice?.DiscountedPriceWithVat ?? 0;

            clonedWorkProduct.PricingInfo = ConstructPricingInfoForTrade(clubPricing);
            SetProductSaleConfigurationForTrade(clonedWorkProduct, clubPricing?.Prices);
        }

        private static PricingInfo ConstructPricingInfoForTrade(ClubPricing clubPricing)
        {
            if (clubPricing == null) return null;

            var defaultPrice = clubPricing.DefaultPrice;

            var result = new PricingInfo
            {
                DefaultPrice = defaultPrice.ToCoreDataProductPrice(),
                AvailablePrices = clubPricing.Prices?.Select(ToCoreDataProductPrice).ToList(),
                DiscountOfferInfo = clubPricing.DiscountOfferInfo == null ? null : ConvertToCoreDataContracts(clubPricing.DiscountOfferInfo)
            };

            return result;
        }


        /// <summary>
        /// Pulsen Service ProductPrice is mapped to CoreData ProductPrice Contract
        /// </summary>
        /// <param name="pulsenPrice"></param>
        /// <returns></returns>
        public static ProductPrice ToCoreDataProductPrice(this PulsenServices.Api.Contracts.Product.ProductPrice pulsenPrice)
        {
            if (pulsenPrice == null) return null;

            return new ProductPrice
            {
                ValidFrom = pulsenPrice.ValidFrom,
                ValidTo = pulsenPrice.ValidTo,
                PriceWithVat = pulsenPrice.PriceWithVat,
                PriceWithoutVat = pulsenPrice.PriceWithoutVat,
                DiscountPercentage = pulsenPrice.DiscountPercentage,
                DiscountedPriceWithVat = pulsenPrice.DiscountedPriceWithVat,
                VatPercentage = pulsenPrice.VatPercentage,
                DiscountedPriceWithoutVat = pulsenPrice.DiscountedPriceWithoutVat,
                PriceType = ResolvePriceTypeForTrade(pulsenPrice.PriceType)
            };
        }


        public static Contracts.Models.DiscountOfferInfo ConvertToCoreDataContracts(Gyldendal.PulsenServices.Api.Contracts.Product.DiscountOfferInfo input)
        {
            if (input == null) return null;
            return new Contracts.Models.DiscountOfferInfo
            {
                OfferStartDate = input.OfferStartDate,
                OfferEndDate = input.OfferEndDate,
                OfferMiniText = input.OfferMiniText,
                OfferLongText = input.OfferLongText,
                IsbnsPartOfOffer = input.IsbnsPartOfOffer,
            };
        }

        private static List<Area> GetWorkAreasForTrade(PulsenProduct pulsenProdDetail)
        {
            if (pulsenProdDetail.ProjectIds == null || !pulsenProdDetail.ProjectIds.Any()) return new List<Area>();
            return pulsenProdDetail.ProjectIds.Select(x => new Area
            {
                Id = (int)x.Key,
                Name = x.Value,
                WebShop = x.Key.GetGyldendalWebShop()
            }).ToList();
        }
        private static void SetProductSaleConfigurationForTrade(Product clonedWorkProduct, List<PulsenServices.Api.Contracts.Product.ProductPrice> pulsenFixedPrices)
        {
            if (clonedWorkProduct.SalesConfiguration == null)
            {
                clonedWorkProduct.SalesConfiguration = new ShopServices.Contracts.SalesConfiguration.SalesConfiguration();
            }
            clonedWorkProduct.SalesConfiguration.FixedPrices = new List<ShopServices.Contracts.SalesConfiguration.FixedPrice>();
            clonedWorkProduct.SalesConfiguration.Approved = true;

            if (pulsenFixedPrices == null)
            {
                return;
            }

            clonedWorkProduct.SalesConfiguration.FixedPrices.AddRange(pulsenFixedPrices.Select(pulsenPrice => new ShopServices.Contracts.SalesConfiguration.FixedPrice
            {
                Price = pulsenPrice.PriceWithVat,
                ValidFrom = pulsenPrice.ValidFrom,
                ValidTo = pulsenPrice.ValidTo,
                FixedPriceType = ResolveFixedPriceTypeForTrade(pulsenPrice.PriceType)
            }));
        }

        private static ShopServices.Contracts.SalesConfiguration.FixedPriceType ResolveFixedPriceTypeForTrade(PriceType fixedPriceType)
        {
            switch (fixedPriceType)
            {
                case PriceType.ClubPrice:
                    return ShopServices.Contracts.SalesConfiguration.FixedPriceType.ClubPrice;

                case PriceType.SpotPrice:
                    return ShopServices.Contracts.SalesConfiguration.FixedPriceType.SpotPrice;

                case PriceType.VipPrice:
                    return ShopServices.Contracts.SalesConfiguration.FixedPriceType.VipPrice;

                case PriceType.BotmPrice:
                    return ShopServices.Contracts.SalesConfiguration.FixedPriceType.BotmPrice;

                default:
                    throw new InvalidOperationException($"No Price type mapping exists for price type {fixedPriceType}");
            }
        }

        private static ProductPriceType ResolvePriceTypeForTrade(PriceType priceType)
        {
            switch (priceType)
            {
                case PriceType.ClubPrice:
                    return ProductPriceType.ClubPrice;

                case PriceType.SpotPrice:
                    return ProductPriceType.SpotPrice;

                case PriceType.VipPrice:
                    return ProductPriceType.VipPrice;

                case PriceType.BotmPrice:
                    return ProductPriceType.BotmPrice;

                case PriceType.UnknownPrice:
                    return ProductPriceType.Default;

                default:
                    throw new InvalidOperationException($"No Price type mapping exists for price type {priceType}");
            }
        }
    }
}
