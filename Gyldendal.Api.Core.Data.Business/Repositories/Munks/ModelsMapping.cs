using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common;
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

namespace Gyldendal.Api.CoreData.Business.Repositories.Munks
{
    public static class ModelsMapping
    {
        /// <summary>
        /// Creates Work Object Using DEA_KDWS_MUNKproduct
        /// </summary>
        /// <param name="munkProduct"></param>
        /// <param name="prodCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Work ToCoreDataWork(this DEA_KDWS_MUNKproduct munkProduct, DEA_KDWS_MUNKcategory[] prodCategories,
            ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil,
            IConfigurationManager configManager, bool isSupplementaryMaterial = false)
        {
            var product = munkProduct.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager, isSupplementaryMaterial);
            var work = new Work
            {
                Id = munkProduct.DEA_KDWS_MUNKwork.work_id,
                Title = munkProduct.DEA_KDWS_MUNKwork.titel,
                WebShop = WebShop.MunksGaard,
                Levels = munkProduct.GetLevels(),
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
        /// Creates Work Object using DEA_KDWS_MUNKBundle Object
        /// </summary>
        /// <param name="munkBundle"></param>
        /// <param name="munkCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Work ToCoreDataWork(this DEA_KDWS_MUNKBundle munkBundle, DEA_KDWS_MUNKcategory[] munkCategories, ShopServices.ApiClient.Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var product = munkBundle.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager);

            var work = new Work
            {
                Id = munkBundle.work_id ?? 0,
                Title = munkBundle.titel,
                WebShop = WebShop.MunksGaard,
                Levels = munkBundle.DEA_KDWS_MUNKBundle_Products.GetLevels(),
                Areas = munkCategories.GetAreas(),
                Subjects = munkCategories.GetSubjects(),
                SubAreas = munkCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null,
                ThemaCodes = null
            };

            return work;
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_MUNKproduct Object
        /// </summary>
        /// <param name="munkProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Product ToCoreDataProduct(this DEA_KDWS_MUNKproduct munkProduct, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial)
        {
            var variantImage = imageUtil.GetProductImagesVariant(munkProduct.ISBN13, DataScope.MunksGaardShop);
            var product = new Product
            {
                Id = munkProduct.vare_id,
                Isbn13 = munkProduct.ISBN13,
                Title = munkProduct.titel,
                Subtitle = munkProduct.undertitel,
                Description = munkProduct.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = munkProduct.materialetype, WebShop = WebShop.MunksGaard },
                MediaType = new MediaType { Name = munkProduct.medietype, WebShop = WebShop.MunksGaard },
                WorkId = munkProduct.work_id,
                PublishDate = munkProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = munkProduct.udgivelsesdato,
                SampleUrl = munkProduct.ReadingSamples,
                SeoText = munkProduct.SEO_Text,
                Edition = munkProduct.udgave,
                Pages = munkProduct.sider,
                ExcuseCode = munkProduct.undskyldningskode,
                Publisher = munkProduct.forlag,
                DurationInMinutes = munkProduct.spilletid.ToInt(),
                InStock = munkProduct.lagertal != null && munkProduct.lagertal > 0,
                Series = munkProduct.GetSeries(configManager),
                Contributors = munkProduct.GetContributors(),
                IsPublished = munkProduct.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = munkProduct.IsNextPrintRunPlanned,
                CoverImages = variantImage.ProductImages,
                OriginalCoverImageUrl = variantImage.OriginalCoverImageUrl,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(munkProduct.medietype.ToLower()),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.MunksGaard,
                        productId = munkProduct.vare_id,
                        mediaType = munkProduct.medietype,
                        isPhysical = configManager.PhysicalMediaTypes.Contains(munkProduct.medietype.ToLower()),
                        url = munkProduct.Url,
                        hasAttachments = munkProduct.DEA_KDWS_MUNKattachments.Any(x => x.is_secured == true && x.kd_slettet == 0),
                        configManager = configManager
                    }),

                LastUpdated = munkProduct.LastUpdated,
                WebShop = WebShop.MunksGaard,
                ProductType = ProductType.SingleProduct,
                IsSupplementaryMaterial = isSupplementaryMaterial,
                Reviews = munkProduct.GetReviews(),
                InspectionCopyAllowed = munkProduct.gennemsynseksemplar.GetValueOrDefault(0) > 0,
                MaterialTypeRank = munkProduct.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(munkProduct.medietype),
                GrossWeight = munkProduct.Gross_weight.GetValueOrDefault(),
                NetWeight = munkProduct.Net_weight.GetValueOrDefault(),
                Height = munkProduct.Height.GetValueOrDefault(),
                Width = munkProduct.Width.GetValueOrDefault(),
                ThicknessDepth = munkProduct.Thickness_depth.GetValueOrDefault(),
                Distributors = null, // Might be filled in future.
                ProductSource = ProductSource.Rap,
                Imprint = munkProduct.Imprint
            };
            product.SetProductSalesConfiguration(munkProduct.pris_uden_moms.GetValueOrDefault(0),
               munkProduct.pris_med_moms.GetValueOrDefault(0), shopServicesApiClient);
            //SetProductSalesConfiguration(munkProduct, shopServicesApiClient, product);
            // Set the Last updated date to Maximum of Product last updated date or Sales configuration Created date
            if (product.SalesConfiguration != null && product.SalesConfiguration.CreatedDate > product.LastUpdated)
            { product.LastUpdated = product.SalesConfiguration.CreatedDate; }

            var discountProductPrice = munkProduct.GetProductDiscount(shopServicesApiClient);

            if (discountProductPrice != null)
            { product.DiscountPercentage = discountProductPrice.DiscountPercentage; }

            if (product.IsPhysical)
            {
                product.SetProductDefaultPriceInfo(munkProduct.pris_med_moms.GetValueOrDefault(0),
                    munkProduct.pris_uden_moms.GetValueOrDefault(0), discountProductPrice);
            }
         

            product.ExtendedPurchaseOptions = munkProduct.GetExtendedPurchaseOption(product.IsPhysical);

            return product;
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_munkBundle Object
        /// </summary>
        /// <param name="munkBundle"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Product ToCoreDataProduct(this DEA_KDWS_MUNKBundle munkBundle, ShopServices.ApiClient.Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var bundleProducts = ToCoreDataBundleProduct(munkBundle.DEA_KDWS_MUNKBundle_Products, shopServicesApiClient, configManager);

            var product = new Product
            {
                Id = munkBundle.bundle_id,
                Isbn13 = munkBundle.ISBN13,
                Title = munkBundle.titel,
                Subtitle = munkBundle.undertitel,
                Description = munkBundle.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = munkBundle.materialetype, WebShop = WebShop.MunksGaard },
                MediaType = new MediaType { Name = munkBundle.medietype, WebShop = WebShop.MunksGaard },
                WorkId = munkBundle.work_id,
                PublishDate = munkBundle.udgivelsesdato,
                SampleUrl = munkBundle.Url,
                SeoText = munkBundle.SEO_Text,
                Edition = munkBundle.udgave,
                Pages = munkBundle.sider,
                ExcuseCode = munkBundle.undskyldningskode,
                Publisher = munkBundle.forlag,
                DurationInMinutes = munkBundle.spilletid.ToInt(),
                InStock = bundleProducts.All(x => !x.IsPhysical || (x.IsPhysical && x.InStock)),
                IsPublished = munkBundle.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = false,
                OriginalCoverImageUrl = munkBundle.illustrationURL,
                IsPhysical = bundleProducts.All(x => x.IsPhysical),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.MunksGaard,
                        productId = munkBundle.bundle_id,
                        mediaType = munkBundle.medietype,
                        isPhysical = true,
                        url = munkBundle.Url,
                        hasAttachments = false,
                        configManager = configManager
                    }), // Bundles are considered as physical product and with no attachments
                LastUpdated = munkBundle.LastUpdated,
                WebShop = WebShop.MunksGaard,
                ProductType = ProductType.Bundle,
                BundleProducts = bundleProducts,
                MembershipPaths = munkBundle.DEA_KDWS_MUNKBundleMembership.GetMembershipPaths(),
                MaterialTypeRank = munkBundle.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(munkBundle.medietype),

                Distributors = null, // Might be filled in future.
                Series = null,
                Contributors = null,
                Reviews = null,
                CoverImages = null
            };

            return product;
        }

        /// <summary>
        /// Get CoreData BundleProduct
        /// </summary>
        /// <param name="kdBundleProducts"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<BundleProduct> ToCoreDataBundleProduct(this ICollection<DEA_KDWS_MUNKBundle_Products> kdBundleProducts,
            ShopServices.ApiClient.Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var bundleProducts = kdBundleProducts?.Select(x => new BundleProduct
            {
                DiscountPercentage = x.discount_percentage ?? 0m,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(x.DEA_KDWS_MUNKproduct.medietype.ToLower()),
                Isbn = x.DEA_KDWS_MUNKproduct.ISBN13,
                Title = x.DEA_KDWS_MUNKproduct.titel,
                SalesConfiguration = x.DEA_KDWS_MUNKproduct.GetBundleProductSaleConfiguration(shopServicesApiClient, configManager),
                InStock = x.DEA_KDWS_MUNKproduct.lagertal != null && x.DEA_KDWS_MUNKproduct.lagertal > 0,
                IsPublished = x.DEA_KDWS_MUNKproduct.udgivelsesdato <= DateTime.Now,
                ProductPrices = null,
                MediaType = new MediaType
                {
                    Name = x.DEA_KDWS_MUNKproduct.medietype,
                    WebShop = WebShop.MunksGaard
                },
                MaterialType = new MaterialType
                {
                    Name = x.DEA_KDWS_MUNKproduct.materialetype,
                    WebShop = WebShop.MunksGaard
                },
                IsNextPrintRunPlanned = x.DEA_KDWS_MUNKproduct.IsNextPrintRunPlanned,
                CurrentPrintRunPublishDate = x.DEA_KDWS_MUNKproduct.udgivelsesdato,
                PublishDate = x.DEA_KDWS_MUNKproduct.FirstPrintPublishDate,
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
        /// <param name="munkProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static SalesConfiguration GetBundleProductSaleConfiguration(this DEA_KDWS_MUNKproduct munkProduct,
            ShopServices.ApiClient.Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var isPhysical = configManager.PhysicalMediaTypes.Contains(munkProduct.medietype.ToLower());
            if (isPhysical)
            {
                return new SalesConfiguration().SetPhysicalBundleProductSaleConfiguration(munkProduct.ISBN13, munkProduct.pris_med_moms.GetValueOrDefault(0),
                    munkProduct.pris_uden_moms.GetValueOrDefault(0), munkProduct.LastUpdated);
            }
            else
            {
                return shopServicesApiClient.SalesConfiguration.GetSalesConfiguration(munkProduct.ISBN13, WebShop.MunksGaard);
            }
        }

        /// <summary>
        /// Get product's discounts from ShopServices.
        /// </summary>
        /// <param name="munkProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <returns></returns>
        private static DiscountProductPrice GetProductDiscount(this DEA_KDWS_MUNKproduct munkProduct, ShopServices.ApiClient.Client shopServicesApiClient)
        {
            var parameters = new List<DiscountParameters>
            {
                new DiscountParameters()
                {
                    ShopName = WebShop.MunksGaard,
                    ProductId = munkProduct.ISBN13,
                    MediaType = munkProduct.medietype,
                    CampaignCode = "",
                    ItemQuantity = 0,
                    DiscountPercentage = null,
                    InputPrice = munkProduct.pris_uden_moms ?? 0,
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
        /// Initialize the extended purchase options.
        /// </summary>
        /// <param name="munkproduct"></param>
        /// <param name="isPhysical"></param>
        /// <returns></returns>
        private static List<ExtendedPurchaseOption> GetExtendedPurchaseOption(this DEA_KDWS_MUNKproduct munkproduct, bool isPhysical)
        {
            var extendedPurchaseOptions = new List<ExtendedPurchaseOption>();

            if (!Constants.HrmIsbnWithoutTeacherSamplePurchaseOption.Contains(munkproduct.ISBN13) && isPhysical)
            {
                extendedPurchaseOptions.Add(new ExtendedPurchaseOption
                {
                    Type = PurchaseOptionType.TeacherCopy,
                    Description = PurchaseOptionType.TeacherCopy.GetDescription(),
                    PurchaseOptionProperties = new PurchaseOptionProperties
                    {
                        ProductMaxQuantity = 1
                    }
                });
            }

            if (munkproduct.gennemsynseksemplar.GetValueOrDefault(0) > 0)
            {
                extendedPurchaseOptions.Add(new ExtendedPurchaseOption
                {
                    Type = PurchaseOptionType.InspectionCopy,
                    Description = PurchaseOptionType.InspectionCopy.GetDescription(),
                    PurchaseOptionProperties = new PurchaseOptionProperties
                    {
                        ProductMaxQuantity = 1
                    }
                });
            }

            return extendedPurchaseOptions;
        }

        /// <summary>
        /// Get MembershipPath
        /// </summary>
        /// <param name="kdMembershipPaths"></param>
        /// <returns></returns>
        private static List<string> GetMembershipPaths(this ICollection<DEA_KDWS_MUNKBundleMembership> kdMembershipPaths)
        {
            return kdMembershipPaths?
                .Select(x => x.MembershipPath)
                .ToList();
        }

        /// <summary>
        /// Gets the List  of ProductReview objects for the review text related to munks Product.
        /// </summary>
        /// <param name="munkProduct"></param>
        /// <returns></returns>
        private static List<ProductReview> GetReviews(this DEA_KDWS_MUNKproduct munkProduct)
        {
            var reviews = munkProduct.DEA_KDWS_MUNKProductReviews.ToList();

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
        /// Get Levels of Products
        /// </summary>
        /// <param name="munkProduct"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this DEA_KDWS_MUNKproduct munkProduct)
        {
            return munkProduct.DEA_KDWS_MUNKProductLevels.Select(pl => new Level
            {
                Name = pl.DEA_KDWS_MUNKlevel.navn,
                LevelNumber = pl.DEA_KDWS_MUNKlevel.niveau
            }).ToList();
        }

        /// <summary>
        /// Get Levels of Products
        /// </summary>
        /// <param name="munkProducts"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this ICollection<DEA_KDWS_MUNKBundle_Products> munkProducts)
        {
            if (munkProducts == null || munkProducts.Any() == false)
                return null;

            var levels = new List<Level>();
            foreach (var munkProduct in munkProducts)
            {
                levels.AddRange(munkProduct.DEA_KDWS_MUNKproduct.GetLevels());
            }

            return levels;
        }

        /// <summary>
        /// Get Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Area> GetAreas(this DEA_KDWS_MUNKcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(4);

            if (categories != null && categories.Any())
            {
                return
                    categories.Select(x => new Area { Id = x.Item1, Name = x.Item2, WebShop = WebShop.MunksGaard })
                        .ToList();
            }

            return new Area[] { }.ToList();
        }

        /// <summary>
        /// Get Subjects of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Subject> GetSubjects(this DEA_KDWS_MUNKcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(5);

            if (categories != null && categories.Any())
            {
                return
                    categories.Select(
                        x => new Subject { Id = x.Item1, Name = x.Item2, AreaId = x.Item3, WebShop = WebShop.MunksGaard })
                        .ToList();
            }

            return new Subject[] { }.ToList();
        }

        /// <summary>
        /// Get Sub Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<SubArea> GetSubAreas(this DEA_KDWS_MUNKcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(6);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new SubArea
                {
                    Id = x.Item1,
                    Name = x.Item2,
                    WebShop = WebShop.MunksGaard,
                    SubjectId = x.Item3.GetValueOrDefault(0)
                }).ToList();
            }

            return new SubArea[] { }.ToList();
        }

        /// <summary>
        /// Get Series of Product and return list of series
        /// </summary>
        /// <param name="munkProduct"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<Series> GetSeries(this DEA_KDWS_MUNKproduct munkProduct, IConfigurationManager configManager)
        {
            return munkProduct.DEA_KDWS_MUNKproductseries.Select(x => x.DEA_KDWS_MUNKseries)
                .Select(x => new Series
                {
                    Id = x.id,
                    Name = x.navn,
                    WebShop = WebShop.MunksGaard,
                    ImageUrl = Common.ModelsMapping.GetSystemSeriesImageUrl(x.id, configManager, x.Is_Image_Uploaded),
                    LastUpdated = x.LastUpdated,
                    IsSystemSeries = x.Type == 1,
                    ParentSerieId = x.parent_id
                }).ToList();
        }

        /// <summary>
        /// Get Product Contributors
        /// </summary>
        /// <param name="munkProduct"></param>
        /// <returns></returns>
        private static List<Contributor> GetContributors(this DEA_KDWS_MUNKproduct munkProduct)
        {
            return munkProduct.DEA_KDWS_MUNKproductcontributors.OrderBy(c => c.firmpers_stamdata_sortorder).Select(c => new Contributor
            {
                Id = c.DEA_KDWS_MUNKContributors.contributor_id,
                AuthorNumber = c.DEA_KDWS_MUNKContributors.Forfatternr,
                FirstName = c.DEA_KDWS_MUNKContributors.contributor_fornavn,
                LastName = c.DEA_KDWS_MUNKContributors.contributor_efternavn,
                Photo = c.DEA_KDWS_MUNKContributors.contributor_foto,
                Url = c.DEA_KDWS_MUNKContributors.contributor_profileLink,
                ContibutorType = (ContributorType)c.role_id
            }).ToList();
        }

        /// <summary>
        /// Extracts and returns a Tuple of int (CategoryId), string (Category Name), and int? (Parent category Id), from the passed in list of category objects and the given level.
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static List<Tuple<int, string, int?>> GetCategoriesByLevel(this DEA_KDWS_MUNKcategory[] prodCategories,
            int level)
        {
            var categories =
                prodCategories.Where(x => x.niveau == level)
                    .Select(x => new Tuple<int, string, int?>(x.id, x.navn.Trim(), x.parent))
                    .ToList();

            return categories;
        }

        public static ContributorDetails ToCoreDataContributor(this DEA_KDWS_MUNKContributors kdContributors)
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
                ContibutorType = kdContributors.DEA_KDWS_MUNKproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ",
                    kdContributors.DEA_KDWS_MUNKproductcontributors.Select(x => x.DEA_KDWS_MUNKproduct)
                        .Select(x => x.titel)
                        .Take(3)),
                Biography = kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.MunksGaard }
            };

            return contributor;
        }

        public static ContributorDetailsV2 ToCoreDataContributorV2(this DEA_KDWS_MUNKContributors kdContributors, List<ProfileImage> photos)
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
                ContibutorType = kdContributors.DEA_KDWS_MUNKproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ",
                    kdContributors.DEA_KDWS_MUNKproductcontributors.Select(x => x.DEA_KDWS_MUNKproduct)
                        .Select(x => x.titel)
                        .Take(3)),
                Biography = kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.MunksGaard }
            };

            return contributor;
        }

        public static Contributor ToCoreDataContributor(this DEA_KDWS_MUNKContributors kdContributors,
            ContributorType type)
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
                    string.Join(", ",
                        kdContributors.DEA_KDWS_MUNKproductcontributors.Select(x => x.DEA_KDWS_MUNKproduct)
                            .Select(x => x.titel)
                            .Take(3)),
            };

            return contributor;
        }
    }
}