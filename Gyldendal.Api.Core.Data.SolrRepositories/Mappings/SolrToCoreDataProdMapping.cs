using System;
using System.Collections.Generic;
using System.Linq;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using NewRelic.Api.Agent;
using MaterialType = Gyldendal.Api.CoreData.Contracts.Models.MaterialType;
using MediaType = Gyldendal.Api.CoreData.Contracts.Models.MediaType;

namespace Gyldendal.Api.CoreData.SolrDataProviders.Mappings
{
    /// <summary>
    /// Extensions methods on SolrContracts.Product.Product class, mainly to construct and return the objects from Core Data Contracts.
    /// </summary>
    public static class SolrProductExtensions
    {
        /// <summary>
        /// Creates and returns CoreData Work object using the passed in group of Solr product objects.
        /// </summary>
        /// <param name="solrProdsGroup"></param>
        /// <returns></returns>
        [Trace]
        public static Contracts.Models.Work ToCoreDataWork(this SolrNet.Group<SolrContracts.Product.Product> solrProdsGroup)
        {
            if (solrProdsGroup?.Documents == null || !(solrProdsGroup.Documents.Any()))
            {
                throw new ArgumentException(
                    $"Argument {nameof(solrProdsGroup)} cannot be null and must contain at least one document in the documents collection.");
            }

            var firstProd = solrProdsGroup.Documents.First();

            var coreDataProds = solrProdsGroup.Documents.Select(x => x.ToCoreDataProduct()).ToArray();

            return new Contracts.Models.Work
            {
                Areas = firstProd.GetAreas().ToList(),
                Description = firstProd.WorkText,
                Id = firstProd.WorkId,
                Levels = firstProd.GetLevels().ToList(),
                Products = coreDataProds.ToList(),
                SubAreas = firstProd.GetSubAreas().ToList(),
                Subjects = firstProd.GetSubjects().ToList(),
                ThemaCodes = firstProd.GetThemaCodes().ToList(),
                Title = firstProd.WorkTitle,
                WebShop = (WebShop)firstProd.WebsiteId
            };
        }

        [Trace]
        public static ContributorDetails ToCoreDataContributor(this SolrContracts.Contributor.Contributor contributor)
        {
            if (contributor == null)
            {
                throw new ArgumentException("Argument contributor cannot be null.");
            }

            var images = DeserializeString(contributor.ImagesJson, new List<ProfileImage>());
            var profileImageUrl = images.FirstOrDefault(i => i.Type.Equals("Profile Image"))?.Url;

            return new ContributorDetails
            {
                ContributorName = contributor.ContributorName,
                Bibliography = contributor.Description,
                ContributorFirstName = contributor.FirstName,
                Id = contributor.ContributorId,
                ContributorLastName = contributor.LastName,
                Photo = profileImageUrl,
                WebShopsId = new List<WebShop> { (WebShop)contributor.WebsiteId },
                SearchName = contributor.SearchName
            };
        }

        [Trace]
        public static ContributorDetailsV2 ToCoreDataContributorV2(this SolrContracts.Contributor.Contributor contributor)
        {
            if (contributor == null)
            {
                throw new ArgumentException("Argument contributor cannot be null.");
            }

            var images = DeserializeString(contributor.ImagesJson, new List<ProfileImage>());

            return new ContributorDetailsV2
            {
                ContributorName = contributor.ContributorName,
                Bibliography = contributor.Description,
                ContributorFirstName = contributor.FirstName,
                Id = contributor.ContributorId,
                ContributorLastName = contributor.LastName,
                Photos = images,
                WebShopsId = new List<WebShop> { (WebShop)contributor.WebsiteId },
                SearchName = contributor.SearchName
            };
        }

        /// <summary>
        /// Maps solr WorkReview object to Coredata conracts object
        /// </summary>
        /// <param name="workReview">solr workreview object</param>
        /// <returns>Coredata contracts' WorkReview object</returns>
        [Trace]
        public static Contracts.Models.WorkReview ToCoreDataWorkReview(this SolrContracts.WorkReview.WorkReview workReview)
        {
            if (workReview == null)
            {
                throw new ArgumentException("Argument workReview cannot be null.");
            }

            return new Contracts.Models.WorkReview
            {
                WorkReviewId = workReview.WorkReviewId,
                LastUpdated = workReview.LastUpdated,
                Priority = workReview.Priority,
                Review = workReview.Review,
                ReviewAttributeId = workReview.ReviewAttributeId,
                Draft = workReview.Draft,
                WorkId = workReview.WorkId,
                Rating = workReview.Rating,
                TextType = workReview.TextType,
                ShortDescription = workReview.ShortDescription,
                Title = workReview.Titleda,
                AuthorInfo = workReview.AboutAuthor,
                Id = workReview.Id,
            };
        }

        /// <summary>
        /// Creates and returns CoreData Product object using the passed in Solr Producct object.
        /// </summary>
        /// <param name="solrProd"></param>
        /// <returns></returns>
        [Trace]
        public static Contracts.Models.Product ToCoreDataProduct(this SolrContracts.Product.Product solrProd)
        {
            if (solrProd == null)
            {
                throw new ArgumentException("Argument solrProd cannot be null.");
            }

            var bundleProducts = solrProd.GetBundleProducts();

            return new Contracts.Models.Product
            {
                Contributors = solrProd.GetContributors().ToList(),
                CoverImages = solrProd.GetProductImages().ToList(),
                Description = solrProd.Description,
                Distributors = solrProd.GetDistributors().ToList(),
                DurationInMinutes = solrProd.Duration,
                Edition = solrProd.Edition,
                ExcuseCode = solrProd.ExcuseCode,
                Id = solrProd.ProductId,
                InStock = solrProd.InStock,
                Isbn13 = solrProd.Isbn13,
                IsPublished = solrProd.CurrentPrintRunPublishDate.Date <= DateTime.Now.Date,
                MaterialType = solrProd.GetMaterialType(),
                MediaType = solrProd.GetMediaType(),
                OriginalCoverImageUrl = solrProd.OriginalCoverImageUrl,
                Pages = solrProd.Pages,
                PublishDate = solrProd.PublishDate,
                CurrentPrintRunPublishDate = solrProd.CurrentPrintRunPublishDate,
                Publisher = solrProd.Publisher,
                Reviews = solrProd.GetProductReviews().ToList(),
                SalesConfiguration = solrProd.GetSalesConfiguration(),
                PricingInfo = solrProd.GetPricingInfo(),
                SampleUrl = solrProd.SampleUrl,
                SeoText = solrProd.SeoText,
                Series = solrProd.GetSeries().ToList(),
                Subtitle = solrProd.Subtitle,
                Title = solrProd.Title,
                WorkId = solrProd.WorkId,
                IsPhysical = solrProd.IsPhysical,
                ProductUrls = solrProd.GetProductUrls().ToList(),
                LastUpdated = solrProd.LastUpdated,
                WebShop = (WebShop)solrProd.WebsiteId,
                ProductType = (ProductType)solrProd.ProductType,
                BundleProducts = bundleProducts.ToList(),
                MembershipPaths = solrProd.MembershipPaths?.ToList(),
                InspectionCopyAllowed = solrProd.InspectionCopyAllowed,
                IsSupplementaryMaterial = solrProd.IsSupplementaryMaterial,
                DiscountPercentage = (decimal)solrProd.DiscountPercentage,
                HasOtherDiscount = solrProd.HasOtherDiscount,
                MaterialTypeRank = solrProd.MaterialTypeRank,
                MediaTypeRank = solrProd.MediaTypeRank,
                GrossWeight = solrProd.GrossWeight,
                NetWeight = solrProd.NetWeight,
                Height = solrProd.Height,
                Width = solrProd.Width,
                ThicknessDepth = solrProd.ThicknessDepth,
                FreeMaterials = solrProd.GetProductFreeMaterials().ToList(),
                IsNextPrintPlanned = solrProd.IsNextPrintRunPlanned,
                ProductSource = (ProductSource)solrProd.ProductSource,
                Genres = solrProd.GetGenres().ToList(),
                Categories = solrProd.GetCategories().ToList(),
                SubCategories = solrProd.GetSubCategories().ToList(),
                Labels = solrProd.Labels?.ToList(),
                DefaultPrice = solrProd.DefaultPrice?.Value,
                DiscountedPrice = solrProd.DiscountedPrice?.Value,
                PublisherId = solrProd.PublisherId,
                HasImages = solrProd.HasImages,
                HasVideos = solrProd.HasVideos,
                PhysicalIsbn = solrProd.PhysicalIsbn,
                ExtraData = solrProd.ExtraData,
                Imprint = solrProd.Imprint,
                ExtendedPurchaseOptions = solrProd.GetExtendedPurchaseOptions().ToList(),
            };
        }

        /// <summary>
        /// Deserializes the SerializedExtendedPurchaseOption property into array of ExtendedPurchaseOption objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static ExtendedPurchaseOption[] GetExtendedPurchaseOptions(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.serializedExtendedPurchaseOption, new ExtendedPurchaseOption[] { });
        }

        /// <summary>
        /// Deserializes the SerializedAreasInfo property into array of Area objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static Genre[] GetGenres(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedGenres, new Genre[] { });
        }

        /// <summary>
        /// Deserializes the SerializedAreasInfo property into array of Area objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static Category[] GetCategories(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedCategories, new Category[] { });
        }

        /// <summary>
        /// Deserializes the SerializedAreasInfo property into array of Area objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static SubCategory[] GetSubCategories(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedSubcategories, new SubCategory[] { });
        }

        /// <summary>
        /// Deserializes the SerializedAreasInfo property into array of Area objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static Area[] GetAreas(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedAreasInfo, new Area[] { });
        }

        /// <summary>
        /// Deserializes the SerializedLevelsInfo property into array of Level objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static Level[] GetLevels(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedLevelsInfo, new Level[] { });
        }

        /// <summary>
        /// Deserializes the SerializedThemaCodes property into array of ThemaCode objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static ThemaCode[] GetThemaCodes(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedThemaCodes, new ThemaCode[] { });
        }

        /// <summary>
        /// Deserializes the SerializedSubAreasInfo property into array of SubArea objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static SubArea[] GetSubAreas(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedSubAreasInfo, new SubArea[] { });
        }

        /// <summary>
        /// Deserializes the SerializedSubjectsInfo property into array of Subject objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static Subject[] GetSubjects(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedSubjectsInfo, new Subject[] { });
        }

        /// <summary>
        /// Deserializes the SerializedSeriesInfo property into array of Series objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static Series[] GetSeries(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedSerisInfo, new Series[] { });
        }

        /// <summary>
        /// Deserializes the SerializedContributorsInfo property into array of Contributor objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static Contracts.Models.Contributor[] GetContributors(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedContributorsInfo, new Contracts.Models.Contributor[] { });
        }

        /// <summary>
        /// Deserializes the SerializedDistributorsInfo property into array of ElectronicDistributor objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static ElectronicDistributor[] GetDistributors(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedDistributorsInfo, new ElectronicDistributor[] { });
        }

        /// <summary>
        /// Deserializes the SerializedSalesConfigs property into SalesConfiguration object from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static SalesConfiguration GetSalesConfiguration(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerailizedSalesConfigs, (SalesConfiguration)null);
        }

        /// <summary>
        /// Deserializes the SerializedSalesConfigs property into SalesConfiguration object from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static PricingInfo GetPricingInfo(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedPricingInfo, (PricingInfo)null);
        }

        /// <summary>
        /// Deserializes the SerializedReviews property into array of ProductReview objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static ProductReview[] GetProductReviews(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedReviews, new ProductReview[] { });
        }

        /// <summary>
        /// Deserializes the SerializedCoverImagesInfo property into array of ProductImage objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static ProductImage[] GetProductImages(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedCoverImagesInfo, new ProductImage[] { });
        }

        /// <summary>
        /// Deserializes the serializedProductUrlInfo property into array of ProductUrl objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static ProductUrl[] GetProductUrls(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.serializedProductUrlInfo, new ProductUrl[] { });
        }

        /// <summary>
        /// Deserializes the serializedProductFreeMaterial property into array of ProductFreeMaterial objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static ProductFreeMaterial[] GetProductFreeMaterials(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.serializedProductFreeMaterial, new ProductFreeMaterial[] { });
        }

        /// <summary>
        /// Deserializes the SerializedBundledProducts property into array of BundleProduct objects from core data contracts.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static BundleProduct[] GetBundleProducts(this SolrContracts.Product.Product product)
        {
            return DeserializeString(product.SerializedBundledProducts, new BundleProduct[] { });
        }

        /// <summary>
        /// Ues own properties to construct and return the core data contract's MediaType object.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static MediaType GetMediaType(this SolrContracts.Product.Product product)
        {
            return new MediaType { Name = product.MediaTypeName, WebShop = (WebShop)product.WebsiteId };
        }

        /// <summary>
        /// Ues own properties to construct and return the core data contract's MaterialType object.
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        [Trace]
        private static MaterialType GetMaterialType(this SolrContracts.Product.Product product)
        {
            return new MaterialType { Name = product.MaterialTypeName, WebShop = (WebShop)product.WebsiteId };
        }

        /// <summary>
        /// Utility method to deserialize and XML string to the requested object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <param name="defaultVal">Default value to be returned if the passed in Xml String is empty or if some erorr occurs while deserializing.</param>
        /// <returns></returns>
        [Trace]
        private static T DeserializeString<T>(string xmlString, T defaultVal)
        {
            if (string.IsNullOrWhiteSpace(xmlString))
            {
                return defaultVal;
            }

            try
            {
                var serializedResult = SerializerUtil.Deserialize<T> (xmlString);
                if (serializedResult == null)
                {
                    return defaultVal;
                }

                return serializedResult;
            }
            catch (Exception)
            {
                return defaultVal;
            }
        }
    }
}