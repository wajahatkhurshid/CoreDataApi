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
using Gyldendal.Api.ShopServices.Contracts.Discount;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using MaterialType = Gyldendal.Api.CoreData.Contracts.Models.MaterialType;
using MediaType = Gyldendal.Api.CoreData.Contracts.Models.MediaType;

namespace Gyldendal.Api.CoreData.Business.Repositories.GU
{
    public static class ModelsMapping
    {
        /// <summary>
        /// Creates Work Object Using DEA_KDWS_GUproduct
        /// </summary>
        /// <param name="guProduct"></param>
        /// <param name="prodCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Work ToCoreDataWork(this DEA_KDWS_GUproduct guProduct, DEA_KDWS_GUcategory[] prodCategories, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial = false)
        {
            var product = guProduct.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager, isSupplementaryMaterial);

            var work = new Work
            {
                Id = guProduct.DEA_KDWS_GUwork.work_id,
                Title = guProduct.DEA_KDWS_GUwork.titel,
                WebShop = WebShop.Gu,
                Levels = guProduct.GetLevels(),
                Areas = prodCategories.GetAreas(),
                Subjects = prodCategories.GetSubjects(),
                SubAreas = prodCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null, // Might be filled in future.
                ThemaCodes = null // TODO: Confirm if we need to fill this?
            };

            return work;
        }

        /// <summary>
        /// Creates Work Object using DEA_KDWS_GUBundle Object
        /// </summary>
        /// <param name="guBundle"></param>
        /// <param name="guCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Work ToCoreDataWork(this DEA_KDWS_GUBundle guBundle, DEA_KDWS_GUcategory[] guCategories, ShopServices.ApiClient.Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var product = guBundle.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager);

            var work = new Work
            {
                Id = guBundle.work_id ?? 0,
                Title = guBundle.titel,
                WebShop = WebShop.Gu,
                Levels = guBundle.DEA_KDWS_GUBundle_Products.GetLevels(),
                Areas = guCategories.GetAreas(),
                Subjects = guCategories.GetSubjects(),
                SubAreas = guCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null,
                ThemaCodes = null
            };

            return work;
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_GUproduct Object
        /// </summary>
        /// <param name="guProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Product ToCoreDataProduct(this DEA_KDWS_GUproduct guProduct, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial)
        {
            var variantImage = imageUtil.GetProductImagesVariant(guProduct.ISBN13, DataScope.GuShop);
            var product = new Product
            {
                Id = guProduct.vare_id,
                Isbn13 = guProduct.ISBN13,
                Title = guProduct.titel,
                Subtitle = guProduct.undertitel,
                Description = guProduct.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = guProduct.materialetype, WebShop = WebShop.Gu },
                MediaType = new MediaType { Name = guProduct.medietype, WebShop = WebShop.Gu },
                WorkId = guProduct.work_id,
                PublishDate = guProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = guProduct.udgivelsesdato,
                SampleUrl = guProduct.ReadingSamples,
                SeoText = guProduct.SEO_Text,
                Edition = guProduct.udgave,
                Pages = guProduct.sider,
                ExcuseCode = guProduct.undskyldningskode,
                Publisher = guProduct.forlag,
                DurationInMinutes = guProduct.spilletid.ToInt(),
                InStock = guProduct.lagertal != null && guProduct.lagertal > 0,
                Series = guProduct.GetSeries(configManager),
                Contributors = guProduct.GetContributors(),
                IsPublished = guProduct.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = guProduct.IsNextPrintRunPlanned,
                CoverImages = variantImage.ProductImages,
                OriginalCoverImageUrl = variantImage.OriginalCoverImageUrl,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(guProduct.medietype.ToLower()),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.Gu,
                        productId = guProduct.vare_id,
                        mediaType = guProduct.medietype,
                        isPhysical = configManager.PhysicalMediaTypes.Contains(guProduct.medietype.ToLower()),
                        url = guProduct.Url,
                        hasAttachments = guProduct.DEA_KDWS_GUattachments.Any(x => x.is_secured == true && x.kd_slettet == 0),
                        configManager = configManager
                    }),

                LastUpdated = guProduct.LastUpdated,
                WebShop = WebShop.Gu,
                Reviews = guProduct.GetReviews(),
                ProductType = ProductType.SingleProduct,
                IsSupplementaryMaterial = isSupplementaryMaterial,
                MaterialTypeRank = guProduct.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(guProduct.medietype),
                GrossWeight = guProduct.Gross_weight.GetValueOrDefault(),
                NetWeight = guProduct.Net_weight.GetValueOrDefault(),
                Height = guProduct.Height.GetValueOrDefault(),
                Width = guProduct.Width.GetValueOrDefault(),
                ThicknessDepth = guProduct.Thickness_depth.GetValueOrDefault(),
                Distributors = null, // Might be filled in future.
                InspectionCopyAllowed = guProduct.gennemsynseksemplar.GetValueOrDefault(0) > 0,
                ProductSource = ProductSource.Rap,
                Imprint = guProduct.Imprint
            };
            product.SetProductSalesConfiguration(guProduct.pris_uden_moms.GetValueOrDefault(0),
                guProduct.pris_med_moms.GetValueOrDefault(0), shopServicesApiClient);
            //SetProductSalesConfiguration(guProduct, shopServicesApiClient, product);
            // Set the Last updated date to Maximum of Product last updated date or Sales configuration Created date
            if (product.SalesConfiguration != null && product.SalesConfiguration.CreatedDate > product.LastUpdated)
            { product.LastUpdated = product.SalesConfiguration.CreatedDate; }

            var discountProductPrice = guProduct.GetProductDiscounts(shopServicesApiClient);
            if (discountProductPrice != null)
            { product.DiscountPercentage = discountProductPrice.DiscountPercentage; }

            if (product.IsPhysical)
            {
                product.SetProductDefaultPriceInfo(guProduct.pris_med_moms.GetValueOrDefault(0),
                    guProduct.pris_uden_moms.GetValueOrDefault(0), discountProductPrice);
            }

            return product;
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_GUBundle Object
        /// </summary>
        /// <param name="guBundle"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Product ToCoreDataProduct(this DEA_KDWS_GUBundle guBundle, ShopServices.ApiClient.Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var bundleProducts = ToCoreDataBundleProduct(guBundle.DEA_KDWS_GUBundle_Products, shopServicesApiClient,
                configManager);

            var product = new Product
            {
                Id = guBundle.bundle_id,
                Isbn13 = guBundle.ISBN13,
                Title = guBundle.titel,
                Subtitle = guBundle.undertitel,
                Description = guBundle.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = guBundle.materialetype, WebShop = WebShop.Gu },
                MediaType = new MediaType { Name = guBundle.medietype, WebShop = WebShop.Gu },
                WorkId = guBundle.work_id,
                PublishDate = guBundle.udgivelsesdato,
                SampleUrl = guBundle.Url,
                SeoText = guBundle.SEO_Text,
                Edition = guBundle.udgave,
                Pages = guBundle.sider,
                ExcuseCode = guBundle.undskyldningskode,
                Publisher = guBundle.forlag,
                DurationInMinutes = guBundle.spilletid.ToInt(),
                InStock = bundleProducts.All(x => !x.IsPhysical || (x.IsPhysical && x.InStock)),
                IsPublished = guBundle.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = false,
                OriginalCoverImageUrl = guBundle.illustrationURL,
                IsPhysical = bundleProducts.All(x => x.IsPhysical),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.Gu,
                        productId = guBundle.bundle_id,
                        mediaType = guBundle.medietype,
                        isPhysical = true,
                        url = guBundle.Url,
                        hasAttachments = false,
                        configManager = configManager
                    }), // Bundle are considered as physical and with no attachments
                LastUpdated = guBundle.LastUpdated,
                WebShop = WebShop.Gu,
                ProductType = ProductType.Bundle,
                BundleProducts = bundleProducts,
                MembershipPaths = guBundle.DEA_KDWS_GUBundleMembership.GetMembershipPaths(),
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(guBundle.medietype),
                MaterialTypeRank = guBundle.materialetypeRank.GetValueOrDefault(1),

                Distributors = null, // Might be filled in future.
                Series = null,
                Contributors = null,
                Reviews = null,
                CoverImages = null
            };
            return product;
        }

        public static ContributorDetails ToCoreDataContributor(this DEA_KDWS_GUContributors kdContributors)
        {
            var contributor = new ContributorDetails
            {
                AuthorNumber = kdContributors.Forfatternr,
                ContributorName = kdContributors.contributor_navn,
                ContributorFirstName = kdContributors.contributor_fornavn,
                ContributorLastName = kdContributors.contributor_efternavn,
                Id = kdContributors.contributor_id,
                Photo = kdContributors.contributor_foto,
                Url = kdContributors.contributor_profileLink,
                ContibutorType = kdContributors.DEA_KDWS_GUproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_GUproductcontributors.Select(x => x.DEA_KDWS_GUproduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography = kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.Gu }
            };

            return contributor;
        }

        public static ContributorDetailsV2 ToCoreDataContributorV2(this DEA_KDWS_GUContributors kdContributors, List<ProfileImage> photos)
        {
            var contributor = new ContributorDetailsV2
            {
                AuthorNumber = kdContributors.Forfatternr,
                ContributorName = kdContributors.contributor_navn,
                ContributorFirstName = kdContributors.contributor_fornavn,
                ContributorLastName = kdContributors.contributor_efternavn,
                Id = kdContributors.contributor_id,
                Photos = photos,
                Url = kdContributors.contributor_profileLink,
                ContibutorType = kdContributors.DEA_KDWS_GUproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_GUproductcontributors.Select(x => x.DEA_KDWS_GUproduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography = kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.Gu }
            };

            return contributor;
        }

        public static Contributor ToCoreDataContributor(this DEA_KDWS_GUContributors kdContributors, ContributorType type)
        {
            var contributor = new Contributor
            {
                AuthorNumber = kdContributors.Forfatternr,
                FirstName = kdContributors.contributor_fornavn,
                LastName = kdContributors.contributor_efternavn,
                Id = kdContributors.contributor_id,
                Photo = kdContributors.contributor_foto,
                Url = kdContributors.contributor_profileLink,
                ContibutorType = type,
                Bibliography = kdContributors.contributor_information,
                Biography =
                    string.Join(", ", kdContributors.DEA_KDWS_GUproductcontributors.Select(x => x.DEA_KDWS_GUproduct)
                        .Select(x => x.titel)
                        .Take(3)),
            };

            return contributor;
        }

        /// <summary>
        /// Get product's discounts from ShopServices.
        /// </summary>
        /// <param name="guProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <returns></returns>
        private static DiscountProductPrice GetProductDiscounts(this DEA_KDWS_GUproduct guProduct, ShopServices.ApiClient.Client shopServicesApiClient)
        {
            var parameters = new List<DiscountParameters>
            {
                new DiscountParameters()
                {
                    ShopName = WebShop.Gu,
                    ProductId = guProduct.ISBN13,
                    MediaType = guProduct.medietype,
                    CampaignCode = "",
                    ItemQuantity = 1,
                    DiscountPercentage = null,
                    InputPrice = guProduct.pris_uden_moms ?? 0,
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

        /// <summary>
        /// Get CoreData BundleProduct
        /// </summary>
        /// <param name="kdBundleProducts"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<BundleProduct> ToCoreDataBundleProduct(this ICollection<DEA_KDWS_GUBundle_Products> kdBundleProducts,
            ShopServices.ApiClient.Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var bundleProducts = kdBundleProducts?.Select(x => new BundleProduct
            {
                DiscountPercentage = x.discount_percentage ?? 0m,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(x.DEA_KDWS_GUproduct.medietype.ToLower()),
                Isbn = x.DEA_KDWS_GUproduct.ISBN13,
                Title = x.DEA_KDWS_GUproduct.titel,
                SalesConfiguration = x.DEA_KDWS_GUproduct.GetBundleProductSaleConfiguration(shopServicesApiClient, configManager),
                InStock = x.DEA_KDWS_GUproduct.lagertal != null && x.DEA_KDWS_GUproduct.lagertal > 0,
                IsPublished = x.DEA_KDWS_GUproduct.udgivelsesdato <= DateTime.Now,
                MediaType = new MediaType
                {
                    Name = x.DEA_KDWS_GUproduct.medietype,
                    WebShop = WebShop.Gu
                },
                MaterialType = new MaterialType
                {
                    Name = x.DEA_KDWS_GUproduct.materialetype,
                    WebShop = WebShop.Gu
                },
                ProductPrices = null,
                IsNextPrintRunPlanned = x.DEA_KDWS_GUproduct.IsNextPrintRunPlanned,
                CurrentPrintRunPublishDate = x.DEA_KDWS_GUproduct.udgivelsesdato,
                PublishDate = x.DEA_KDWS_GUproduct.FirstPrintPublishDate,
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

        /// <summary>
        /// Setting bundleProduct's sale configuration
        /// </summary>
        /// <param name="guProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static SalesConfiguration GetBundleProductSaleConfiguration(this DEA_KDWS_GUproduct guProduct,
            ShopServices.ApiClient.Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var isPhysical = configManager.PhysicalMediaTypes.Contains(guProduct.medietype.ToLower());
            if (isPhysical)
            {
                return new SalesConfiguration().SetPhysicalBundleProductSaleConfiguration(guProduct.ISBN13, guProduct.pris_med_moms.GetValueOrDefault(0),
                    guProduct.pris_uden_moms.GetValueOrDefault(0), guProduct.LastUpdated);
            }
            else
            {
                return shopServicesApiClient.SalesConfiguration.GetSalesConfiguration(guProduct.ISBN13, WebShop.Gu);
            }
        }

        /// <summary>
        /// Get MembershipPath
        /// </summary>
        /// <param name="kdMembershipPaths"></param>
        /// <returns></returns>
        private static List<string> GetMembershipPaths(this ICollection<DEA_KDWS_GUBundleMembership> kdMembershipPaths)
        {
            return kdMembershipPaths?
                .Select(x => x.MembershipPath)
                .ToList();
        }

        /// <summary>
        /// Get Sub Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<SubArea> GetSubAreas(this DEA_KDWS_GUcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(3);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new SubArea
                {
                    Id = x.Item1,
                    Name = x.Item2,
                    WebShop = WebShop.Gu,
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
        private static List<Subject> GetSubjects(this DEA_KDWS_GUcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(2);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Subject { Id = x.Item1, Name = x.Item2, AreaId = x.Item3, WebShop = WebShop.Gu, }).ToList();
            }

            return new Subject[] { }.ToList();
        }

        /// <summary>
        /// Get Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Area> GetAreas(this DEA_KDWS_GUcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(1);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Area { Id = x.Item1, Name = x.Item2, WebShop = WebShop.Gu }).ToList();
            }

            return new Area[] { }.ToList();
        }

        /// <summary>
        /// Get Levels of Product
        /// </summary>
        /// <param name="guProduct"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this DEA_KDWS_GUproduct guProduct)
        {
            return guProduct.DEA_KDWS_GUProductLevels.Select(pl => new Level
            {
                Name = pl.DEA_KDWS_GUlevel.navn,
                LevelNumber = pl.DEA_KDWS_GUlevel.niveau
            }).ToList();
        }

        /// <summary>
        /// Get Levels of Products
        /// </summary>
        /// <param name="guProducts"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this ICollection<DEA_KDWS_GUBundle_Products> guProducts)
        {
            if (guProducts == null || guProducts.Any() == false)
                return null;

            var levels = new List<Level>();
            foreach (var guProduct in guProducts)
            {
                levels.AddRange(guProduct.DEA_KDWS_GUproduct.GetLevels());
            }

            return levels;
        }

        /// <summary>
        /// Get Series of Product and return list of series
        /// </summary>
        /// <param name="guProduct"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<Series> GetSeries(this DEA_KDWS_GUproduct guProduct, IConfigurationManager configManager)
        {
            return guProduct.DEA_KDWS_GUproductseries.Select(x => x.DEA_KDWS_GUseries)
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
        /// Gets the List  of ProductReview objects for the review text related to Gu Product.
        /// </summary>
        /// <param name="guProduct"></param>
        /// <returns></returns>
        private static List<ProductReview> GetReviews(this DEA_KDWS_GUproduct guProduct)
        {
            var reviews = guProduct.DEA_KDWS_GUProductReviews.ToList();

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
        /// <param name="guProduct"></param>
        /// <returns></returns>
        private static List<Contributor> GetContributors(this DEA_KDWS_GUproduct guProduct)
        {
            return guProduct.DEA_KDWS_GUproductcontributors.OrderBy(c => c.firmpers_stamdata_sortorder).Select(c => new Contributor
            {
                Id = c.DEA_KDWS_GUContributors.contributor_id,
                AuthorNumber = c.DEA_KDWS_GUContributors.Forfatternr,
                FirstName = c.DEA_KDWS_GUContributors.contributor_fornavn,
                LastName = c.DEA_KDWS_GUContributors.contributor_efternavn,
                Photo = c.DEA_KDWS_GUContributors.contributor_foto,
                Url = c.DEA_KDWS_GUContributors.contributor_profileLink,
                ContibutorType = (ContributorType)c.role_id
            }).ToList();
        }

        /// <summary>
        /// Extracts and returns a Tuple of int (CategoryId), string (Category Name), and int? (Parent category Id), from the passed in list of category objects and the given level.
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static List<Tuple<int, string, int?>> GetCategoriesByLevel(this DEA_KDWS_GUcategory[] prodCategories, int level)
        {
            var categories =
                prodCategories.Where(x => x.niveau == level)
                .Select(x => new Tuple<int, string, int?>(x.id, x.navn.Trim(), x.parent))
                .ToList();

            return categories;
        }
    }
}