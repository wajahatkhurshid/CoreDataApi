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
using Gyldendal.Api.ShopServices.Contracts.Discount;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gyldendal.Api.CoreData.Business.Repositories.RosinanteCo
{
    public static class ModelsMapping
    {
        /// <summary>
        /// Creates Work Object Using DEA_KDWS_ROSCOproduct
        /// </summary>
        /// <param name="rosCoProduct"></param>
        /// <param name="prodCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Work ToCoreDataWork(this DEA_KDWS_ROSCOproduct rosCoProduct, DEA_KDWS_ROSCOcategory[] prodCategories, Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial = false)
        {
            var product = rosCoProduct.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager, isSupplementaryMaterial);

            var work = new Work
            {
                Id = rosCoProduct.DEA_KDWS_ROSCOwork.work_id,
                Title = rosCoProduct.DEA_KDWS_ROSCOwork.titel,
                WebShop = WebShop.Rosinanteco,
                Areas = prodCategories.GetAreas(),
                Subjects = prodCategories.GetSubjects(),
                SubAreas = prodCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null, // Might be filled in future.
                ThemaCodes = rosCoProduct.GetThemaCodes()
            };

            return work;
        }

        internal static Product ToCoreDataProduct(this DEA_KDWS_ROSCOBundle roscoBundle,
            Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var bundleProducts = ToCoreDataBundleProduct(roscoBundle.DEA_KDWS_ROSCOBundle_Products, shopServicesApiClient,
                configManager);
            var product = new Product
            {
                Id = roscoBundle.bundle_id,
                Isbn13 = roscoBundle.ISBN13,
                Title = roscoBundle.titel,
                Subtitle = roscoBundle.undertitel,
                Description = roscoBundle.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = roscoBundle.materialetype, WebShop = WebShop.Rosinanteco },
                MediaType = new MediaType { Name = roscoBundle.medietype, WebShop = WebShop.Rosinanteco },
                WorkId = roscoBundle.work_id,
                PublishDate = roscoBundle.udgivelsesdato,
                SampleUrl = roscoBundle.Url,
                SeoText = roscoBundle.SEO_Text,
                Edition = roscoBundle.udgave,
                Pages = roscoBundle.sider,
                ExcuseCode = roscoBundle.undskyldningskode,
                Publisher = roscoBundle.forlag,
                DurationInMinutes = roscoBundle.spilletid.ToInt(),
                InStock = bundleProducts.All(x => !x.IsPhysical || (x.IsPhysical && x.InStock)),
                IsNextPrintPlanned = false,
                IsPublished = roscoBundle.udgivelsesdato <= DateTime.Now,
                OriginalCoverImageUrl = roscoBundle.illustrationURL,
                IsPhysical = bundleProducts.All(x => x.IsPhysical),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.Rosinanteco,
                        productId = roscoBundle.bundle_id,
                        mediaType = roscoBundle.medietype,
                        isPhysical = true,
                        url = roscoBundle.Url,
                        hasAttachments = false,
                        configManager = configManager
                    }),// bundles are considered as physical with no attachments
                LastUpdated = roscoBundle.LastUpdated,
                WebShop = WebShop.Rosinanteco,
                ProductType = ProductType.Bundle,
                BundleProducts = bundleProducts,
                MembershipPaths = roscoBundle.DEA_KDWS_ROSCOBundleMembership.GetMembershipPaths(),
                Distributors = null, // Might be filled in future.
                Series = null,
                Contributors = null,
                Reviews = null,
                CoverImages = null,
                MaterialTypeRank = roscoBundle.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = roscoBundle.medietypeRank.GetValueOrDefault(1),
            };
            return product;
        }

        internal static Product ToCoreDataProduct(this DEA_KDWS_ROSCOproduct roscoProduct, Client shopServicesApiClient, ICoverImageUtil imageUtil,
            IConfigurationManager configManager, bool isSupplementaryMaterial)
        {
            var variantImage = imageUtil.GetProductImagesVariant(roscoProduct.ISBN13, DataScope.RosinantecoShop);
            var product = new Product
            {
                Id = roscoProduct.vare_id,
                Isbn13 = roscoProduct.ISBN13,
                Title = roscoProduct.titel,
                Subtitle = roscoProduct.undertitel,
                Description = roscoProduct.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = roscoProduct.materialetype, WebShop = WebShop.Rosinanteco },
                MediaType = new MediaType { Name = roscoProduct.medietype, WebShop = WebShop.Rosinanteco },
                WorkId = roscoProduct.work_id,
                PublishDate = roscoProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = roscoProduct.udgivelsesdato,
                SampleUrl = roscoProduct.ReadingSamples,
                SeoText = roscoProduct.SEO_Text,
                Edition = roscoProduct.udgave,
                Pages = roscoProduct.sider,
                ExcuseCode = roscoProduct.ErrorCode,
                Publisher = roscoProduct.redaktion,
                DurationInMinutes = roscoProduct.spilletid.ToInt(),
                InStock = roscoProduct.lagertal != null && roscoProduct.lagertal > 0,
                Series = roscoProduct.GetSeries(configManager),
                Contributors = roscoProduct.GetContributors(),
                IsPublished = roscoProduct.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = roscoProduct.IsNextPrintRunPlanned,
                CoverImages = variantImage.ProductImages,
                OriginalCoverImageUrl = roscoProduct.forside,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(roscoProduct.medietype.ToLower()),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.Rosinanteco,
                        productId = roscoProduct.vare_id,
                        mediaType = roscoProduct.medietype,
                        isPhysical = configManager.PhysicalMediaTypes.Contains(roscoProduct.medietype.ToLower()),
                        url = roscoProduct.Url,
                        hasAttachments = roscoProduct.DEA_KDWS_ROSCOattachments.Any(x => x.is_secured == true && x.kd_slettet == 0),
                        configManager = configManager
                    }),

                LastUpdated = roscoProduct.LastUpdated,
                WebShop = WebShop.Rosinanteco,
                Reviews = roscoProduct.GetReviews(),
                ProductType = ProductType.SingleProduct,
                IsSupplementaryMaterial = isSupplementaryMaterial,
                Distributors = null, // Might be filled in future.
                InspectionCopyAllowed = roscoProduct.gennemsynseksemplar.GetValueOrDefault(0) > 0,
                MaterialTypeRank = roscoProduct.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = roscoProduct.medietypeRank.GetValueOrDefault(1),
                ProductSource = ProductSource.Gyldendal
            };

            var priceWithoutTax = roscoProduct.vejledende_pris ?? 0;
            var priceWithTax = roscoProduct.pris_med_moms ?? 0;

            product.SetProductSaleConfigurationOnFly(priceWithTax, priceWithoutTax);
            // Set the Last updated date to Maximum of Product last updated date or Sales configuration Created date
            if (product.SalesConfiguration != null && product.SalesConfiguration.CreatedDate > product.LastUpdated)
            {
                product.LastUpdated = product.SalesConfiguration.CreatedDate;
            }

            var parameters = new List<DiscountParameters>
            {
                new DiscountParameters
                {
                    ShopName = WebShop.Rosinanteco,
                    ProductId = roscoProduct.ISBN13,
                    MediaType = roscoProduct.medietype,
                    CampaignCode = "",
                    ItemQuantity = 1,
                    DiscountPercentage = null,
                    InputPrice = priceWithoutTax,
                    ProductScope = 0,
                    MembershipPaths = null,
                    AccessFormCode = 0,
                    UnitValue = 0,
                    RefPeriodUnitTypeCode = 0
                }
            };
            var discountprice = shopServicesApiClient.Discount.CalculateDiscount(parameters);
            var discountProductPrice = discountprice.FirstOrDefault();
            if (discountProductPrice != null)
                product.DiscountPercentage = discountProductPrice.DiscountPercentage;
            return product;
        }

        /// <summary>
        /// Get CoreData BundleProduct
        /// </summary>
        /// <param name="kdBundleProducts"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<BundleProduct> ToCoreDataBundleProduct(this ICollection<DEA_KDWS_ROSCOBundle_Products> kdBundleProducts,
            IClient shopServicesApiClient, IConfigurationManager configManager)
        {
            var bundleProducts = kdBundleProducts?.Select(x => new BundleProduct
            {
                DiscountPercentage = x.discount_percentage ?? 0m,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(x.DEA_KDWS_ROSCOproduct.medietype.ToLower()),
                Isbn = x.DEA_KDWS_ROSCOproduct.ISBN13,
                Title = x.DEA_KDWS_ROSCOproduct.titel,
                SalesConfiguration = x.DEA_KDWS_ROSCOproduct.GetBundleProductSaleConfiguration(shopServicesApiClient, configManager),
                InStock = x.DEA_KDWS_ROSCOproduct.lagertal != null && x.DEA_KDWS_ROSCOproduct.lagertal > 0,
                IsPublished = x.DEA_KDWS_ROSCOproduct.udgivelsesdato <= DateTime.Now,
                MediaType = new MediaType
                {
                    Name = x.DEA_KDWS_ROSCOproduct.medietype,
                    WebShop = WebShop.Rosinanteco
                },
                MaterialType = new MaterialType
                {
                    Name = x.DEA_KDWS_ROSCOproduct.materialetype,
                    WebShop = WebShop.Rosinanteco
                },
                ProductPrices = null,
                IsNextPrintRunPlanned = x.DEA_KDWS_ROSCOproduct.IsNextPrintRunPlanned,
                CurrentPrintRunPublishDate = x.DEA_KDWS_ROSCOproduct.udgivelsesdato,
                PublishDate = x.DEA_KDWS_ROSCOproduct.FirstPrintPublishDate,
            }).ToList();
            if (ValidateBundle.ValidateBundleProducts(bundleProducts))
            {
                bundleProducts?.ForEach(x => x.ProductPrices = x.SalesConfiguration.CalculateBundlePrice(x.DiscountPercentage));
            }
            else
            {
                bundleProducts?.ForEach(x => x.SalesConfiguration = null);
            }

            return bundleProducts;
        }

        private static List<ProductReview> GetReviews(this DEA_KDWS_ROSCOproduct roscoProduct)
        {
            var reviews = roscoProduct.DEA_KDWS_ROSCOProductReviews.ToList();

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
        /// <param name="roscoProduct"></param>
        /// <returns></returns>
        private static List<Contributor> GetContributors(this DEA_KDWS_ROSCOproduct roscoProduct)
        {
            return roscoProduct.DEA_KDWS_ROSCOproductcontributors
                .OrderBy(o => o.firmpers_stamdata_sortorder).Select(c => new Contributor
                {
                    Id = c.DEA_KDWS_ROSCOContributors.contributor_id.UpdateAuthorId(c.role_id),
                    AuthorNumber = c.DEA_KDWS_ROSCOContributors.Forfatternr,
                    FirstName = c.DEA_KDWS_ROSCOContributors.contributor_fornavn,
                    LastName = c.DEA_KDWS_ROSCOContributors.contributor_efternavn,
                    Photo = c.DEA_KDWS_ROSCOContributors.contributor_foto,
                    Url = c.DEA_KDWS_ROSCOContributors.contributor_profileLink,
                    ContibutorType = (ContributorType)c.role_id
                }).ToList();
        }

        /// <summary>
        /// Get Series of Product and return list of series
        /// </summary>
        /// <param name="roscoProduct"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<Series> GetSeries(this DEA_KDWS_ROSCOproduct roscoProduct, IConfigurationManager configManager)
        {
            return roscoProduct.DEA_KDWS_ROSCOproductseries.Select(x => x.DEA_KDWS_ROSCOseries)
                .Select(x => new Series
                {
                    Id = x.id,
                    Name = x.navn,
                    WebShop = WebShop.Rosinanteco,
                    LastUpdated = x.LastUpdated,
                    ImageUrl = Common.ModelsMapping.GetSystemSeriesImageUrl(x.id, configManager, x.Is_Image_Uploaded),
                    IsSystemSeries = x.Type == 1,
                    ParentSerieId = x.parent_id
                }).ToList();
        }

        /// <summary>
        /// Setting bundleProduct's sale configuration
        /// </summary>
        /// <param name="roscoProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static SalesConfiguration GetBundleProductSaleConfiguration(this DEA_KDWS_ROSCOproduct roscoProduct,
            IClient shopServicesApiClient, IConfigurationManager configManager)
        {
            var priceWithoutVat = roscoProduct.vejledende_pris ?? 0;
            var priceWithVat = roscoProduct.pris_med_moms ?? 0;
            return new SalesConfiguration().SetPhysicalBundleProductSaleConfiguration(roscoProduct.ISBN13,
                priceWithVat,
                priceWithoutVat, roscoProduct.LastUpdated);
        }

        /// <summary>
        /// Get MembershipPath
        /// </summary>
        /// <param name="kdMembershipPaths"></param>
        /// <returns></returns>
        private static List<string> GetMembershipPaths(this ICollection<DEA_KDWS_ROSCOBundleMembership> kdMembershipPaths)
        {
            return kdMembershipPaths?
                .Select(x => x.MembershipPath)
                .ToList();
        }

        private static List<ThemaCode> GetThemaCodes(this DEA_KDWS_ROSCOproduct roscoProduct)
        {
            return roscoProduct.DEA_KDWS_ROSCOProductThemacode.Select(x => new ThemaCode
            {
                Code = x.DEA_KDWS_ROSCOThemacode.ThemaCode,
                Description = x.DEA_KDWS_ROSCOThemacode.DanishDescription,
                Id = x.ThemaCodeId
            }).ToList();
        }

        /// <summary>
        /// Extracts and returns a Tuple of int (CategoryId), string (Category Name), and int? (Parent category Id), from the passed in list of category objects and the given level.
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static List<Tuple<int, string, int?>> GetCategoriesByLevel(this DEA_KDWS_ROSCOcategory[] prodCategories, int level)
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
        private static List<SubArea> GetSubAreas(this DEA_KDWS_ROSCOcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(3);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new SubArea
                {
                    Id = x.Item1,
                    Name = x.Item2,
                    WebShop = WebShop.Rosinanteco,
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
        private static List<Subject> GetSubjects(this DEA_KDWS_ROSCOcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(2);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Subject { Id = x.Item1, Name = x.Item2, AreaId = x.Item3, WebShop = WebShop.Rosinanteco, }).ToList();
            }

            return new Subject[] { }.ToList();
        }

        /// <summary>
        /// Get Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Area> GetAreas(this DEA_KDWS_ROSCOcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(1);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Area { Id = x.Item1, Name = x.Item2, WebShop = WebShop.Rosinanteco }).ToList();
            }

            return new Area[] { }.ToList();
        }

        /// <summary>
        /// Creates Work Object using DEA_KDWS_GUBundle Object
        /// </summary>
        /// <param name="roscoBundle"></param>
        /// <param name="roscoCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Work ToCoreDataWork(this DEA_KDWS_ROSCOBundle roscoBundle, DEA_KDWS_ROSCOcategory[] roscoCategories, Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var product = roscoBundle.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager);

            var work = new Work
            {
                Id = roscoBundle.work_id ?? 0,
                Title = roscoBundle.titel,
                WebShop = WebShop.Rosinanteco,
                Areas = roscoCategories.GetAreas(),
                Subjects = roscoCategories.GetSubjects(),
                SubAreas = roscoCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null,
                ThemaCodes = null,
            };

            return work;
        }

        public static ContributorDetails ToCoreDataContributor(this DEA_KDWS_ROSCOContributors kdContributors)
        {
            var contributor = new ContributorDetails
            {
                AuthorNumber = kdContributors.Forfatternr,
                ContributorName = kdContributors.contributor_navn,
                Id = kdContributors.contributor_id,
                Photo = kdContributors.contributor_foto,
                Url = kdContributors.contributor_profileLink,
                ContibutorType = kdContributors.DEA_KDWS_ROSCOproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = kdContributors.contributor_information,
                Biography =
                    string.Join(", ", kdContributors.DEA_KDWS_ROSCOproductcontributors.Select(x => x.DEA_KDWS_ROSCOproduct)
                        .Select(x => x.titel)
                        .Take(3)),
            };

            return contributor;
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
                Rating = kdWorkReview.Rating,
                Review = kdWorkReview.anmeldelse,
                ReviewAttributeId = kdWorkReview.ReviewAttributeId,
                LastUpdated = kdWorkReview.Lastupdated,
                ShortDescription = kdWorkReview.kortOmBogen,
                TextType = kdWorkReview.teksttype,
                Title = kdWorkReview.anmeldelse,
            };

            return workReview;
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