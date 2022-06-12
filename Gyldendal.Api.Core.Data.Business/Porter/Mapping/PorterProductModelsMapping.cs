using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.ShopServices.Contracts.Discount;
using Contributor = Gyldendal.Api.CoreData.Contracts.Models.Contributor;
using MaterialType = Gyldendal.Api.CoreData.Contracts.Models.MaterialType;
using MediaType = Gyldendal.Api.CoreData.Contracts.Models.MediaType;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Product = Gyldendal.Api.CoreData.Contracts.Models.Product;
using ProductType = Gyldendal.Api.CoreData.Contracts.Enumerations.ProductType;
using Series = Gyldendal.Api.CoreData.Contracts.Models.Series;
using WebShop = Gyldendal.Api.CommonContracts.WebShop;

namespace Gyldendal.Api.CoreData.Business.Porter.Mapping
{
    public static class PorterProductModelsMapping
    {
        private const string MediaproviderUrlVersionPart = "/v1/MediaProvider/GetDigitalMaterial";

        /// <summary>
        /// Creates Product Object using Porter Product Object
        /// </summary>
        /// <param name="porterProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="webShop"></param>
        /// <returns></returns>
        internal static Product ToCoreDataProduct(this PorterApi.Product porterProduct, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, WebShop webShop)
        {
            var variantImage = imageUtil.GetProductImagesVariant(porterProduct.Isbn);
            var product = new Product
            {
                Id = porterProduct.Id,
                Isbn13 = porterProduct.Isbn,
                Title = porterProduct.Title,
                Subtitle = porterProduct.Subtitle,
                Description = porterProduct.Description.RepairHtml(),
                MaterialType = new MaterialType { Name = porterProduct.MaterialType, WebShop = webShop },
                MediaType = new MediaType { Name = porterProduct.MediaType, WebShop = webShop },
                WorkId = porterProduct.WorkId,
                PublishDate = porterProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = porterProduct.CurrentPrintRunPublishDate,
                SampleUrl = porterProduct.ReadingSamples,
                SeoText = porterProduct.SeoText,
                Edition = porterProduct.Edition,
                Pages = porterProduct.NoOfPages,
                ExcuseCode = porterProduct.ExcuseCode,
                Publisher = porterProduct.Publisher,
                DurationInMinutes = porterProduct.DurationInMinutes.ToInt(),
                InStock = porterProduct.Stock != null && porterProduct.Stock > 0,
                Series = porterProduct.GetSeries(webShop),
                Contributors = porterProduct.GetContributors(),
                IsPublished = porterProduct.CurrentPrintRunPublishDate <= DateTime.Now,
                IsNextPrintPlanned = porterProduct.IsNextPrintRunPlanned,
                CoverImages = variantImage.ProductImages,
                OriginalCoverImageUrl = variantImage.OriginalCoverImageUrl,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(porterProduct.MediaType.ToLower()),
                ProductUrls = ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = webShop,
                        productId = porterProduct.Id,
                        mediaType = porterProduct.MediaType,
                        isPhysical = configManager.PhysicalMediaTypes.Contains(porterProduct.MediaType.ToLower()),
                        url = porterProduct.Url,
                        hasAttachments = porterProduct.Attachments.Any(x => x.IsSecured && !x.IsDeleted),
                        configManager = configManager
                    }),
                LastUpdated = porterProduct.UpdatedTimestamp,
                WebShop = webShop,
                Reviews = porterProduct.GetReviews(), //TODO: ?
                ProductType = ProductType.SingleProduct,
                MaterialTypeRank = porterProduct.MaterialTypeRank.GetValueOrDefault(1),
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(porterProduct.MediaType),
                GrossWeight = porterProduct.GrossWeight.GetValueOrDefault(),
                NetWeight = porterProduct.NetWeight.GetValueOrDefault(),
                Height = porterProduct.Height.GetValueOrDefault(),
                Width = porterProduct.Width.GetValueOrDefault(),
                ThicknessDepth = porterProduct.ThicknessDepth.GetValueOrDefault(),
                Distributors = null, // Might be filled in future.
                InspectionCopyAllowed = porterProduct.InspectionCopyAllowed,
                ProductSource = ProductSource.Rap, // TODO: ?
                Imprint = porterProduct.Imprint,
                FreeMaterials = porterProduct.GetFreeMaterials(),
                IsSupplementaryMaterial = porterProduct.Attachments.Any(s => s.IsSecured == false && s.IsDeleted == false)
            };

            product.SetProductSalesConfiguration(
                (decimal)porterProduct.PriceWithoutVat.GetValueOrDefault(0),
                (decimal)porterProduct.PriceWithVat.GetValueOrDefault(0),
                shopServicesApiClient);

            // Set the Last updated date to Maximum of Product last updated date or Sales configuration Created date
            if (product.SalesConfiguration != null && product.SalesConfiguration.CreatedDate > product.LastUpdated)
            {
                product.LastUpdated = product.SalesConfiguration.CreatedDate;
            }

            var discountProductPrice = porterProduct.GetProductDiscounts(
                webShop,
                shopServicesApiClient);

            if (discountProductPrice != null)
            {
                product.DiscountPercentage = discountProductPrice.DiscountPercentage;
            }

            if (product.IsPhysical)
            {
                product.SetProductDefaultPriceInfo(
                    (decimal)porterProduct.PriceWithVat.GetValueOrDefault(0),
                    (decimal)porterProduct.PriceWithoutVat.GetValueOrDefault(0),
                    discountProductPrice);
            }

            return product;
        }

        /// <summary>
        /// Create BundleProduct from CoreDataProduct 
        /// </summary>
        /// <param name="coreDataProduct"></param>
        /// <returns>BundleProduct</returns>
        internal static BundleProduct ToCoreDataBundleProduct(this Product coreDataProduct)
        {
            return new BundleProduct()
            {

                Isbn = coreDataProduct.Isbn13,
                Title = coreDataProduct.Title,
                DiscountPercentage = coreDataProduct.DiscountPercentage.HasValue ? coreDataProduct.DiscountPercentage.Value : 0,
                SalesConfiguration = coreDataProduct.SalesConfiguration,
                ProductPrices = null,
                IsPhysical = coreDataProduct.IsPhysical,
                InStock = coreDataProduct.InStock,
                IsBuyable = coreDataProduct.IsBuyable,
                IsPublished = coreDataProduct.IsPublished,
                CurrentPrintRunPublishDate = coreDataProduct.CurrentPrintRunPublishDate,
                PublishDate = coreDataProduct.PublishDate,
                IsNextPrintRunPlanned = coreDataProduct.IsNextPrintPlanned,
                MaterialType = coreDataProduct.MaterialType,
                MediaType = coreDataProduct.MediaType,
                ProductSource = coreDataProduct.ProductSource
            };
        }

        /// <summary>
        /// Gets the List of ProductSeries objects
        /// </summary>
        /// <param name="product"></param>
        /// <param name="webShop"></param>
        /// <returns></returns>
        internal static List<Series> GetSeries(this PorterApi.Product product, WebShop webShop)
        {
            return product.Series
                .Select(x => new Series
                {
                    Id = x.Id.ToInt(),
                    Name = x.Name,
                    WebShop = webShop,
                    LastUpdated = x.UpdatedTimestamp,
                    ImageUrl = x.ImageUrl,
                    IsSystemSeries = x.IsSystemSeries,
                    ParentSerieId = x.ParentSerieId
                }).ToList();
        }

        /// <summary>
        /// Gets the List of ProductContributors objects
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        internal static List<Contributor> GetContributors(this PorterApi.Product product)
        {
            return product.Contributors.Select(c => new Contributor
            {
                Id = c.Id,
                AuthorNumber = string.Empty, // Its missing on Porter side
                FirstName = c.FirstName,
                LastName = c.LastName,
                Photo = string.Empty,  // Its missing on Porter side
                Url = c.PhotoUrl,
                ContibutorType = (ContributorType)c.ContributorTypeId,
                Biography = c.BiographyText
            }).ToList();
        }

        internal static ContributorDetails ToCoreDataContributors(this PorterApi.GetContributorResponse contributorResponse, WebShop webShop)
        {
            return new ContributorDetails
            {
                Id = contributorResponse.Contributor?.Id,
                AuthorNumber = contributorResponse.Contributor?.FirstName + " " + contributorResponse.Contributor?.LastName,
                Photo = string.Empty,  // Its missing on Porter side
                Url = contributorResponse.Contributor?.PhotoUrl,
                Biography = contributorResponse.Contributor?.BiographyText,
                Bibliography = string.Empty, // Its missing on Porter side
                ContributorFirstName = contributorResponse.Contributor?.FirstName,
                ContributorLastName = contributorResponse.Contributor?.LastName,
                ContributorName = contributorResponse.Contributor?.FirstName + " " + contributorResponse.Contributor?.LastName,
                SearchName = string.Empty,  // Its missing on Porter side
                ContibutorType = new List<ContributorType> { (ContributorType)contributorResponse.Contributor?.ContributorTypeId },
                WebShopsId = contributorResponse.Contributor.WebShops.Select(x => x.ToCoreDataWebShop()).ToList()
            };

        }

        internal static ContributorUpdateInfo ToCoreDataContributors(this PorterApi.GetContributorsUpdateInfoResponse contributorUpdateInfoResponse)
        {
            return new ContributorUpdateInfo
            {
                ContributorId = contributorUpdateInfoResponse.Id,
                UpdateTime = contributorUpdateInfoResponse.UpdateTime,
                UpdateType = contributorUpdateInfoResponse.IsDeleted == true ? ContributorUpdateType.Deleted : ContributorUpdateType.Updated
            };

        }

        /// <summary>
        /// Gets the List of ProductReview objects
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private static List<ProductReview> GetReviews(this PorterApi.Product product)
        {
            return null;
        }

        /// <summary>
        /// Get product's discounts from ShopServices.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="webShop"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <returns></returns>
        private static DiscountProductPrice GetProductDiscounts(this PorterApi.Product product, WebShop webShop, ShopServices.ApiClient.Client shopServicesApiClient)
        {
            var parameters = new List<DiscountParameters>
            {
                new DiscountParameters()
                {
                    ShopName = webShop,
                    ProductId = product.Isbn,
                    MediaType = product.MediaType,
                    CampaignCode = "",
                    ItemQuantity = 1,
                    DiscountPercentage = null,
                    InputPrice = product.PriceWithoutVat.HasValue ? (decimal)product.PriceWithoutVat : 0,
                    ProductScope = 0,
                    MembershipPaths = null,
                    AccessFormCode = 0,
                    UnitValue = 0,
                    RefPeriodUnitTypeCode = 0
                }
            };
            var discountprice = shopServicesApiClient.Discount.CalculateDiscount(parameters);
            var discountProductPrice = discountprice.FirstOrDefault();

            return discountProductPrice;
        }


        private static List<ProductFreeMaterial> GetFreeMaterials(this PorterApi.Product product)
        {
            var freeMaterials = new List<ProductFreeMaterial>();

            var attachments = product.Attachments.Where(a => a.IsDeleted == false && a.IsSecured == false);
            foreach (var attachment in attachments)
            {
                if (string.IsNullOrEmpty(attachment.SampleUrl)) continue;

                var index = attachment.SampleUrl.LastIndexOf('/');
                if (index >= 0)
                {
                    freeMaterials.Add(
                        new ProductFreeMaterial
                        {
                            FileName = attachment.SampleUrl.Substring(index + 1),
                            Description = attachment.Description
                        }
                    );
                }
            }

            return freeMaterials;
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
        public static string GetDigitalProductUrl(WebShop webshop, string productId, string mediaType, string webAddress, IConfigurationManager configManager)
        {
            var eBookMediaTypes = configManager.DownloadableMaterialMediaTypes;

            var mediaProviderBaseUrl = $"{configManager.MediaProviderUrl}{MediaproviderUrlVersionPart}";

            return eBookMediaTypes.Contains(mediaType?.Trim() ?? string.Empty)
                ? $"{mediaProviderBaseUrl}/{productId}/{(int)webshop}"
                : webAddress;
        }


        public static ContributorDetailsV2 ToContributorDetailsV2(this ContributorDetails ccv, List<ProfileImage> photos)
        {
            return new ContributorDetailsV2()
            {
                AuthorNumber = ccv.AuthorNumber,
                Bibliography = ccv.Bibliography,
                Biography = ccv.Biography,
                ContributorName = ccv.ContributorName,
                Photos = photos,
                Id = ccv.Id,
                Url = ccv.Url,
                ContributorFirstName = ccv.ContributorFirstName,
                ContributorLastName = ccv.ContributorLastName,
                WebShopsId = ccv.WebShopsId
            };
        }
    }
}