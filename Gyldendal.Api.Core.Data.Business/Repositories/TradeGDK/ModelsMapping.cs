using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.Api.ShopServices.ApiClient;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;

namespace Gyldendal.Api.CoreData.Business.Repositories.TradeGDK
{
    public static class ModelsMapping
    {
        /// <summary>
        /// Creates Work Object Using DEA_KDWS_GDKproduct
        /// </summary>
        /// <param name="gdkProduct"></param>
        /// <param name="prodCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Work ToCoreDataWork(this DEA_KDWS_GDKProduct gdkProduct, DEA_KDWS_GDKcategory[] prodCategories, Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial = false)
        {
            var product = gdkProduct.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager, isSupplementaryMaterial);

            var work = new Work
            {
                Id = gdkProduct.DEA_KDWS_GDKWork.work_id,
                Title = gdkProduct.DEA_KDWS_GDKWork.titel,
                WebShop = WebShop.TradeGyldendalDk,
                Areas = prodCategories.GetAreas(),
                Subjects = prodCategories.GetSubjects(),
                SubAreas = prodCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null, // Might be filled in future.
                ThemaCodes = gdkProduct.GetThemaCodes()
            };

            return work;
        }

        internal static Product ToCoreDataProduct(this DEA_KDWS_GDKBundle gdkBundle, IConfigurationManager configManager)
        {
            var bundleProducts = ToCoreDataBundleProduct(gdkBundle.DEA_KDWS_GDKBundle_Products, configManager);
            var product = new Product
            {
                Id = gdkBundle.bundle_id,
                Isbn13 = gdkBundle.ISBN13,
                Title = gdkBundle.titel,
                Subtitle = gdkBundle.undertitel,
                Description = gdkBundle.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = gdkBundle.materialetype, WebShop = WebShop.TradeGyldendalDk },
                MediaType = new MediaType { Name = gdkBundle.medietype, WebShop = WebShop.TradeGyldendalDk },
                WorkId = gdkBundle.work_id,
                PublishDate = gdkBundle.udgivelsesdato,
                SampleUrl = gdkBundle.Url,
                SeoText = gdkBundle.SEO_Text,
                Edition = gdkBundle.udgave,
                Pages = gdkBundle.sider,
                ExcuseCode = gdkBundle.undskyldningskode,
                Publisher = gdkBundle.forlag,
                DurationInMinutes = gdkBundle.spilletid.ToInt(),
                InStock = bundleProducts.All(x => !x.IsPhysical || (x.IsPhysical && x.InStock)),
                IsPublished = gdkBundle.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = false,
                OriginalCoverImageUrl = gdkBundle.illustrationURL,
                IsPhysical = bundleProducts.All(x => x.IsPhysical),

                // bundles are considered as physical with no attachments
                ProductUrls = GetProductUrls(gdkBundle.bundle_id, gdkBundle.medietype, gdkBundle.Url, hasAttachments: false, configManager),

                LastUpdated = gdkBundle.LastUpdated,
                WebShop = WebShop.TradeGyldendalDk,
                ProductType = ProductType.Bundle,
                BundleProducts = bundleProducts,
                MembershipPaths = gdkBundle.DEA_KDWS_GDKBundleMembership.GetMembershipPaths(),
                Distributors = null, // Might be filled in future.
                Series = null,
                Contributors = null,
                Reviews = null,
                CoverImages = null,
                MaterialTypeRank = gdkBundle.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = gdkBundle.medietypeRank.GetValueOrDefault(1),
            };
            return product;
        }

        internal static Product ToCoreDataProduct(this DEA_KDWS_GDKProduct gdkProduct, Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial)
        {
            var variantImage = imageUtil.GetProductImagesVariant(gdkProduct.ISBN13, DataScope.TradeGyldendalDk, "GyldendalDk");
            var hasAttachments = gdkProduct.DEA_KDWS_GDKattachments.Any(x => x.is_secured == true && x.kd_slettet == 0);

            var product = new Product
            {
                Id = gdkProduct.vare_id,
                Isbn13 = gdkProduct.ISBN13,
                Title = gdkProduct.titel,
                Subtitle = gdkProduct.undertitel,
                Description = gdkProduct.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = gdkProduct.materialetype, WebShop = WebShop.TradeGyldendalDk },
                MediaType = new MediaType { Name = gdkProduct.medietype, WebShop = WebShop.TradeGyldendalDk },
                WorkId = gdkProduct.work_id,
                PublishDate = gdkProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = gdkProduct.udgivelsesdato,
                SampleUrl = gdkProduct.ReadingSamples,
                SeoText = gdkProduct.SEO_Text,
                Edition = gdkProduct.udgave,
                Pages = gdkProduct.sider,
                ExcuseCode = gdkProduct.ErrorCode,
                Publisher = gdkProduct.redaktion,
                DurationInMinutes = gdkProduct.spilletid.ToInt(),
                InStock = gdkProduct.lagertal != null && gdkProduct.lagertal > 0,
                Series = gdkProduct.GetSeries(configManager),
                Contributors = gdkProduct.GetContributors(),
                IsPublished = gdkProduct.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = gdkProduct.IsNextPrintRunPlanned,
                CoverImages = variantImage.ProductImages,
                OriginalCoverImageUrl = variantImage.OriginalCoverImageUrl,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(gdkProduct.medietype.ToLower()),
                ProductUrls = GetProductUrls(gdkProduct.vare_id, gdkProduct.medietype, gdkProduct.Url, hasAttachments, configManager),
                LastUpdated = gdkProduct.LastUpdated.GetValueOrDefault(),
                WebShop = WebShop.TradeGyldendalDk,
                Reviews = gdkProduct.GetReviews(),
                ProductType = ProductType.SingleProduct,
                IsSupplementaryMaterial = isSupplementaryMaterial,
                MaterialTypeRank = gdkProduct.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = gdkProduct.medietypeRank.GetValueOrDefault(1),
                GrossWeight = gdkProduct.Gross_weight.GetValueOrDefault(),
                NetWeight = gdkProduct.Net_weight.GetValueOrDefault(),
                Height = gdkProduct.Height.GetValueOrDefault(),
                Width = gdkProduct.Width.GetValueOrDefault(),
                ThicknessDepth = gdkProduct.Thickness_depth.GetValueOrDefault(),
                Distributors = null, // Might be filled in future.
                InspectionCopyAllowed = gdkProduct.gennemsynseksemplar.GetValueOrDefault(0) > 0,
                ProductSource = ProductSource.Rap,
                Imprint = gdkProduct.Imprint
            };

            var priceWithoutTax = gdkProduct.vejledende_pris ?? 0;
            var priceWithTax = gdkProduct.pris_med_moms ?? 0;
            product.SetProductSaleConfigurationOnFly(priceWithTax, priceWithoutTax);
            // Set the Last updated date to Maximum of Product last updated date or Sales configuration Created date
            if (product.SalesConfiguration != null && product.SalesConfiguration.CreatedDate > product.LastUpdated)
            {
                product.LastUpdated = product.SalesConfiguration.CreatedDate;
            }

            return product;
        }

        private static List<ProductUrl> GetProductUrls(string productId, string mediaType, string url, bool hasAttachments, IConfigurationManager configManager)
        {
            return Common.ModelsMapping.GetProductUrls(
                new InternalObjects.ProductUrlInput
                {
                    webShop = WebShop.TradeGyldendalDk,
                    productId = productId,
                    mediaType = mediaType,
                    isPhysical = configManager.PhysicalMediaTypes.Contains(mediaType.ToLower()),
                    url = url,
                    hasAttachments = hasAttachments,
                    configManager = configManager
                });
        }

        /// <summary>
        /// Get CoreData BundleProduct
        /// </summary>
        /// <param name="kdBundleProducts"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<BundleProduct> ToCoreDataBundleProduct(this ICollection<DEA_KDWS_GDKBundle_Products> kdBundleProducts, IConfigurationManager configManager)
        {
            var bundleProducts = kdBundleProducts?.Select(x => new BundleProduct
            {
                DiscountPercentage = x.discount_percentage ?? 0m,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(x.DEA_KDWS_GDKProduct.medietype.ToLower()),
                Isbn = x.DEA_KDWS_GDKProduct.ISBN13,
                Title = x.DEA_KDWS_GDKProduct.titel,
                SalesConfiguration = x.DEA_KDWS_GDKProduct.GetBundleProductSaleConfiguration(),
                InStock = x.DEA_KDWS_GDKProduct.lagertal != null && x.DEA_KDWS_GDKProduct.lagertal > 0,
                IsPublished = x.DEA_KDWS_GDKProduct.udgivelsesdato <= DateTime.Now,
                MediaType = new MediaType
                {
                    Name = x.DEA_KDWS_GDKProduct.medietype,
                    WebShop = WebShop.TradeGyldendalDk
                },
                MaterialType = new MaterialType
                {
                    Name = x.DEA_KDWS_GDKProduct.materialetype,
                    WebShop = WebShop.TradeGyldendalDk
                },
                ProductPrices = null,
                IsNextPrintRunPlanned = x.DEA_KDWS_GDKProduct.IsNextPrintRunPlanned,
                CurrentPrintRunPublishDate = x.DEA_KDWS_GDKProduct.udgivelsesdato,
                PublishDate = x.DEA_KDWS_GDKProduct.FirstPrintPublishDate,
            }).ToList();
            if (ValidateBundle.ValidateBundleProducts(bundleProducts))
            {
                bundleProducts?.ForEach(x =>
                    x.ProductPrices = x.SalesConfiguration.CalculateBundlePrice(x.DiscountPercentage));
            }
            else
            {
                bundleProducts?.ForEach(x => x.SalesConfiguration = null);
            }

            return bundleProducts;
        }

        private static List<ProductReview> GetReviews(this DEA_KDWS_GDKProduct gdkProduct)
        {
            var reviews = gdkProduct.DEA_KDWS_GDKProductReviews.ToList();

            if (!reviews.Any())
            {
                return new List<ProductReview>();
            }

            return reviews.Select(x => new ProductReview
            {
                ReviewText = x.tekst,
                ProductId = x.vare,
                ReviewDate = x.Dato ?? DateTime.Now.Date,
                ReviewBy = x.ReviewBy,
                ReviewSource = x.ReviewSource
            }).ToList();
        }

        /// <summary>
        /// Get Product Contributors
        /// </summary>
        /// <param name="gdkProduct"></param>
        /// <returns></returns>
        private static List<Contributor> GetContributors(this DEA_KDWS_GDKProduct gdkProduct)
        {
            return gdkProduct.DEA_KDWS_GDKproductcontributors
                .OrderBy(o => o.firmpers_stamdata_sortorder).Select(c => new Contributor
                {
                    Id = c.DEA_KDWS_GDKContributors.contributor_id.UpdateAuthorId(c.role_id),
                    AuthorNumber = c.DEA_KDWS_GDKContributors.Forfatternr,
                    FirstName = c.DEA_KDWS_GDKContributors.contributor_fornavn,
                    LastName = c.DEA_KDWS_GDKContributors.contributor_efternavn,
                    Photo = c.DEA_KDWS_GDKContributors.contributor_foto,
                    Url = c.DEA_KDWS_GDKContributors.contributor_profileLink,
                    ContibutorType = (ContributorType)c.role_id
                }).ToList();
        }

        /// <summary>
        /// Get Series of Product and return list of series
        /// </summary>
        /// <param name="gdkProduct"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<Series> GetSeries(this DEA_KDWS_GDKProduct gdkProduct, IConfigurationManager configManager)
        {
            return gdkProduct.DEA_KDWS_GDKproductseries.Select(x => x.DEA_KDWS_GDKseries)
                .Select(x => new Series
                {
                    Id = x.id,
                    Name = x.navn,
                    WebShop = WebShop.Gu,
                    LastUpdated = x.LastUpdated,
                    ImageUrl = Common.ModelsMapping.GetSystemSeriesImageUrl(x.id, configManager, x.Is_Image_Uploaded),
                    IsSystemSeries = x.Type == 1,
                    ParentSerieId = x.parent_id
                }).ToList();
        }

        /// <summary>
        /// Setting bundleProduct's sale configuration
        /// </summary>
        /// <param name="gdkProduct"></param>
        /// <returns></returns>
        private static SalesConfiguration GetBundleProductSaleConfiguration(this DEA_KDWS_GDKProduct gdkProduct)
        {
            var priceWithoutVat = gdkProduct.vejledende_pris ?? 0;
            var priceWithVat = gdkProduct.pris_med_moms ?? 0;

            return new SalesConfiguration().SetPhysicalBundleProductSaleConfiguration(gdkProduct.ISBN13,
                priceWithVat,
                priceWithoutVat, gdkProduct.LastUpdated.GetValueOrDefault());
        }

        /// <summary>
        /// Get MembershipPath
        /// </summary>
        /// <param name="kdMembershipPaths"></param>
        /// <returns></returns>
        private static List<string> GetMembershipPaths(this ICollection<DEA_KDWS_GDKBundleMembership> kdMembershipPaths)
        {
            return kdMembershipPaths?
                .Select(x => x.MembershipPath)
                .ToList();
        }

        private static List<ThemaCode> GetThemaCodes(this DEA_KDWS_GDKProduct gdkProduct)
        {
            return gdkProduct.DEA_KDWS_GDKProductThemacode.Select(x => new ThemaCode
            {
                Code = x.DEA_KDWS_GDKThemacode.ThemaCode,
                Description = x.DEA_KDWS_GDKThemacode.DanishDescription,
                Id = x.ThemaCodeId
            }).ToList();
        }

        /// <summary>
        /// Extracts and returns a Tuple of int (CategoryId), string (Category Name), and int? (Parent category Id), from the passed in list of category objects and the given level.
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static List<Tuple<int, string, int?>> GetCategoriesByLevel(this DEA_KDWS_GDKcategory[] prodCategories, int level)
        {
            var categories =
                prodCategories.Where(x => x.niveau == level)
                .Select(x => new Tuple<int, string, int?>(x.id, x.navn.Trim(), x.parent))
                .ToList();

            return categories;
        }

        /// <summary>
        /// Get Sub Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<SubArea> GetSubAreas(this DEA_KDWS_GDKcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(3);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new SubArea
                {
                    Id = x.Item1,
                    Name = x.Item2,
                    WebShop = WebShop.TradeGyldendalDk,
                    SubjectId = x.Item3.GetValueOrDefault(0)
                }).ToList();
            }

            return new SubArea[] { }.ToList();
        }

        /// <summary>
        /// Get Subjects of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Subject> GetSubjects(this DEA_KDWS_GDKcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(2);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Subject { Id = x.Item1, Name = x.Item2, AreaId = x.Item3, WebShop = WebShop.TradeGyldendalDk, }).ToList();
            }

            return new Subject[] { }.ToList();
        }

        /// <summary>
        /// Get Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Area> GetAreas(this DEA_KDWS_GDKcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(1);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Area { Id = x.Item1, Name = x.Item2, WebShop = WebShop.TradeGyldendalDk }).ToList();
            }

            return new Area[] { }.ToList();
        }

        /// <summary>
        /// Creates Work Object using DEA_KDWS_GUBundle Object
        /// </summary>
        /// <param name="gdkBundle"></param>
        /// <param name="gdkCategories"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Work ToCoreDataWork(this DEA_KDWS_GDKBundle gdkBundle, DEA_KDWS_GDKcategory[] gdkCategories, IConfigurationManager configManager)
        {
            var product = gdkBundle.ToCoreDataProduct(configManager);

            var work = new Work
            {
                Id = gdkBundle.work_id ?? 0,
                Title = gdkBundle.titel,
                WebShop = WebShop.TradeGyldendalDk,
                Areas = gdkCategories.GetAreas(),
                Subjects = gdkCategories.GetSubjects(),
                SubAreas = gdkCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null,
                ThemaCodes = null,
            };

            return work;
        }

        public static ContributorDetails ToCoreDataContributor(this DEA_KDWS_GDKContributors kdContributors)
        {
            var contributor = new ContributorDetails
            {
                AuthorNumber = kdContributors.Forfatternr,
                ContributorName = kdContributors.contributor_navn,
                ContributorFirstName = kdContributors.contributor_fornavn,
                ContributorLastName = kdContributors.contributor_efternavn,
                Id = ExtractGdkContributorId(kdContributors.contributor_id),
                Photo = kdContributors.contributor_foto,
                Url = kdContributors.contributor_profileLink,
                ContibutorType = kdContributors.DEA_KDWS_GDKproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_GDKproductcontributors.Select(x => x.DEA_KDWS_GDKProduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography =
                    kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.TradeGyldendalDk }
            };

            return contributor;
        }

        public static ContributorDetailsV2 ToCoreDataContributorV2(this DEA_KDWS_GDKContributors kdContributors, List<ProfileImage> photos)
        {
            var contributor = new ContributorDetailsV2
            {
                AuthorNumber = kdContributors.Forfatternr,
                ContributorName = kdContributors.contributor_navn,
                ContributorFirstName = kdContributors.contributor_fornavn,
                ContributorLastName = kdContributors.contributor_efternavn,
                Id = ExtractGdkContributorId(kdContributors.contributor_id),
                Photos = photos,
                Url = kdContributors.contributor_profileLink,
                ContibutorType = kdContributors.DEA_KDWS_GDKproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_GDKproductcontributors.Select(x => x.DEA_KDWS_GDKProduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography =
                    kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.TradeGyldendalDk }
            };

            return contributor;
        }

        public static string ExtractGdkContributorId(string contributorId)
        {
            if (contributorId.StartsWith("0-"))
            {
                return contributorId.Replace("0-", "F");
            }

            if (contributorId.StartsWith("1-"))
            {
                return contributorId.Replace("1-", "S");
            }

            throw new InvalidDataException("Contributor Id is not valid.");
        }

        public static WorkReview ToCoreDataWorkReview(this ConsolidatedWorkReviewView kdWorkReview)
        {
            var workReview = new WorkReview
            {
                WorkReviewId = kdWorkReview.WorkReviewId,
                WorkId = kdWorkReview.WorkId,
                Id = kdWorkReview.WorkReviewId.ToString(),
                AuthorInfo = kdWorkReview.omForfatteren,
                Draft = kdWorkReview.kladde,
                Rating = FixRatingValueForGyldendalDk(kdWorkReview.Rating),
                Review = kdWorkReview.anmeldelse,
                ReviewAttributeId = kdWorkReview.ReviewAttributeId,
                LastUpdated = kdWorkReview.Lastupdated,
                ShortDescription = kdWorkReview.kortOmBogen,
                TextType = kdWorkReview.teksttype,
                Title = kdWorkReview.anmeldelse,
                WebShopId = WebShop.TradeGyldendalDk
            };

            return workReview;
        }

        private static int FixRatingValueForGyldendalDk(int? ratingFromDb)
        {
            var rating = ratingFromDb.GetValueOrDefault(0);

            if (rating < 0) rating = 0;

            if (rating == 0) return 0;

            return ++rating;
        }

        private static string UpdateAuthorId(this string contributorId, int roleId)
        {
            if ((ContributorType)roleId != ContributorType.Author) return contributorId;

            return contributorId.StartsWith("1-") ?
                Regex.Replace(contributorId, "^1-", "S") :
                Regex.Replace(contributorId, "^0-", "F");
        }
    }
}