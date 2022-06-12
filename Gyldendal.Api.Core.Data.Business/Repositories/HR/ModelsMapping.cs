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
using Gyldendal.Api.ShopServices.ApiClient;
using Gyldendal.Api.ShopServices.Contracts.Discount;
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using MaterialType = Gyldendal.Api.CoreData.Contracts.Models.MaterialType;
using MediaType = Gyldendal.Api.CoreData.Contracts.Models.MediaType;

namespace Gyldendal.Api.CoreData.Business.Repositories.HR
{
    public static class ModelsMapping
    {
        /// <summary>
        /// Creates Work Object Using DEA_KDWS_HRproduct
        /// </summary>
        /// <param name="hrProduct"></param>
        /// <param name="prodCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Work ToCoreDataWork(this DEA_KDWS_HRproduct hrProduct, DEA_KDWS_HRcategory[] prodCategories, Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial = false)
        {
            var product = hrProduct.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager, isSupplementaryMaterial);
            var work = new Work
            {
                Id = hrProduct.DEA_KDWS_HRwork.work_id,
                Title = hrProduct.DEA_KDWS_HRwork.titel,
                WebShop = WebShop.HansReitzel,
                Levels = hrProduct.GetLevels(),
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
        /// Creates Work Object using DEA_KDWS_HRBundle Object
        /// </summary>
        /// <param name="hrBundle"></param>
        /// <param name="hrCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Work ToCoreDataWork(this DEA_KDWS_HRBundle hrBundle, DEA_KDWS_HRcategory[] hrCategories, Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var product = hrBundle.ToCoreDataProduct(shopServicesApiClient, imageUtil, configManager);

            var work = new Work
            {
                Id = hrBundle.work_id ?? 0,
                Title = hrBundle.titel,
                WebShop = WebShop.HansReitzel,
                Levels = hrBundle.DEA_KDWS_HRBundle_Products.GetLevels(),
                Areas = hrCategories.GetAreas(),
                Subjects = hrCategories.GetSubjects(),
                SubAreas = hrCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null,
                ThemaCodes = null
            };

            return work;
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_HRproduct Object
        /// </summary>
        /// <param name="hrProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Product ToCoreDataProduct(this DEA_KDWS_HRproduct hrProduct, Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial)
        {
            var variantImage = imageUtil.GetProductImagesVariant(hrProduct.ISBN13, DataScope.HansReitzelShop);
            var product = new Product
            {
                Id = hrProduct.vare_id,
                Isbn13 = hrProduct.ISBN13,
                Title = hrProduct.titel,
                Subtitle = hrProduct.undertitel,
                Description = hrProduct.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = hrProduct.materialetype, WebShop = WebShop.HansReitzel },
                MediaType = new MediaType { Name = hrProduct.medietype, WebShop = WebShop.HansReitzel },
                WorkId = hrProduct.work_id,
                PublishDate = hrProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = hrProduct.udgivelsesdato,
                SampleUrl = hrProduct.ReadingSamples,
                SeoText = hrProduct.SEO_Text,
                Edition = hrProduct.udgave,
                Pages = hrProduct.sider,
                ExcuseCode = hrProduct.undskyldningskode,
                Publisher = hrProduct.forlag,
                DurationInMinutes = hrProduct.spilletid.ToInt(),
                InStock = hrProduct.lagertal != null && hrProduct.lagertal > 0,
                Series = hrProduct.GetSeries(configManager),
                Contributors = hrProduct.GetContributors(),
                IsPublished = hrProduct.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = hrProduct.IsNextPrintRunPlanned,
                CoverImages = variantImage.ProductImages,
                OriginalCoverImageUrl = variantImage.OriginalCoverImageUrl,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(hrProduct.medietype.ToLower()),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.HansReitzel,
                        productId = hrProduct.vare_id,
                        mediaType = hrProduct.medietype,
                        isPhysical = configManager.PhysicalMediaTypes.Contains(hrProduct.medietype.ToLower()),
                        url = hrProduct.Url,
                        hasAttachments = hrProduct.DEA_KDWS_HRattachments.Any(x => x.is_secured == true && x.kd_slettet == 0),
                        configManager = configManager
                    }),

                LastUpdated = hrProduct.LastUpdated,
                WebShop = WebShop.HansReitzel,
                ProductType = ProductType.SingleProduct,
                IsSupplementaryMaterial = isSupplementaryMaterial,
                Reviews = hrProduct.GetReviews(),
                InspectionCopyAllowed = hrProduct.gennemsynseksemplar.GetValueOrDefault(0) > 0,
                MaterialTypeRank = hrProduct.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(hrProduct.medietype),
                GrossWeight = hrProduct.Gross_weight.GetValueOrDefault(),
                NetWeight = hrProduct.Net_weight.GetValueOrDefault(),
                Height = hrProduct.Height.GetValueOrDefault(),
                Width = hrProduct.Width.GetValueOrDefault(),
                ThicknessDepth = hrProduct.Thickness_depth.GetValueOrDefault(),
                Distributors = null, // Might be filled in future.
                ProductSource = ProductSource.Rap,
                Imprint = hrProduct.Imprint
            };
            product.SetProductSalesConfiguration(hrProduct.pris_uden_moms.GetValueOrDefault(0),
                hrProduct.pris_med_moms.GetValueOrDefault(0), shopServicesApiClient);

            // Set the Last updated date to Maximum of Product last updated date or Sales configuration Created date
            if (product.SalesConfiguration != null && product.SalesConfiguration.CreatedDate > product.LastUpdated)
            { product.LastUpdated = product.SalesConfiguration.CreatedDate; }

            var discountProductPrice = hrProduct.GetProductDiscounts(shopServicesApiClient);

            if (discountProductPrice != null)
            { product.DiscountPercentage = discountProductPrice.DiscountPercentage; }

            if (product.IsPhysical)
            {
                product.SetProductDefaultPriceInfo(hrProduct.pris_med_moms.GetValueOrDefault(0),
                    hrProduct.pris_uden_moms.GetValueOrDefault(0), discountProductPrice);
            }

            product.ExtendedPurchaseOptions = hrProduct.GetExtendedPurchaseOption(product.IsPhysical);

            return product;
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_hrBundle Object
        /// </summary>
        /// <param name="hrBundle"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Product ToCoreDataProduct(this DEA_KDWS_HRBundle hrBundle, Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var bundleProducts = ToCoreDataBundleProduct(hrBundle.DEA_KDWS_HRBundle_Products, shopServicesApiClient,
                configManager);

            var product = new Product
            {
                Id = hrBundle.bundle_id,
                Isbn13 = hrBundle.ISBN13,
                Title = hrBundle.titel,
                Subtitle = hrBundle.undertitel,
                Description = hrBundle.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = hrBundle.materialetype, WebShop = WebShop.HansReitzel },
                MediaType = new MediaType { Name = hrBundle.medietype, WebShop = WebShop.HansReitzel },
                WorkId = hrBundle.work_id,
                PublishDate = hrBundle.udgivelsesdato,
                SampleUrl = hrBundle.Url,
                SeoText = hrBundle.SEO_Text,
                Edition = hrBundle.udgave,
                Pages = hrBundle.sider,
                ExcuseCode = hrBundle.undskyldningskode,
                Publisher = hrBundle.forlag,
                DurationInMinutes = hrBundle.spilletid.ToInt(),
                InStock = bundleProducts.All(x => !x.IsPhysical || (x.IsPhysical && x.InStock)),
                IsPublished = hrBundle.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = false,
                OriginalCoverImageUrl = hrBundle.illustrationURL,
                IsPhysical = bundleProducts.All(x => x.IsPhysical),
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.HansReitzel,
                        productId = hrBundle.bundle_id,
                        mediaType = hrBundle.medietype,
                        isPhysical = true,
                        url = hrBundle.Url,
                        hasAttachments = false,
                        configManager = configManager
                    }), // Bundles are considered as physical product and with no attachments
                LastUpdated = hrBundle.LastUpdated,
                WebShop = WebShop.HansReitzel,
                ProductType = ProductType.Bundle,
                BundleProducts = bundleProducts,
                MembershipPaths = hrBundle.DEA_KDWS_HRBundleMembership.GetMembershipPaths(),
                MaterialTypeRank = hrBundle.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(hrBundle.medietype),

                Distributors = null, // Might be filled in future.
                Series = null,
                Contributors = null,
                Reviews = null,
                CoverImages = null,
            };
            return product;
        }

        /// <summary>
        /// Get product's discounts from ShopServices.
        /// </summary>
        /// <param name="hrProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <returns></returns>
        private static DiscountProductPrice GetProductDiscounts(this DEA_KDWS_HRproduct hrProduct, Client shopServicesApiClient)
        {
            var parameters = new List<DiscountParameters>
            {
                new DiscountParameters()
                {
                    ShopName = WebShop.HansReitzel,
                    ProductId = hrProduct.ISBN13,
                    MediaType = hrProduct.medietype,
                    CampaignCode = "",
                    ItemQuantity = 0,
                    DiscountPercentage = null,
                    InputPrice = hrProduct.pris_uden_moms ?? 0,
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
        /// <param name="hRproduct"></param>
        /// <param name="isPhysical">Type of product</param>
        /// <returns></returns>
        private static List<ExtendedPurchaseOption> GetExtendedPurchaseOption(this DEA_KDWS_HRproduct hRproduct, bool isPhysical)
        {
            var extendedPurchaseOptions = new List<ExtendedPurchaseOption>();

            if (!Constants.HrmIsbnWithoutTeacherSamplePurchaseOption.Contains(hRproduct.ISBN13) && isPhysical)
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

            if (hRproduct.gennemsynseksemplar.GetValueOrDefault(0) > 0)
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
        /// Get Levels of Products
        /// </summary>
        /// <param name="hRproduct"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this DEA_KDWS_HRproduct hRproduct)
        {
            return hRproduct.DEA_KDWS_HRProductLevels.Select(pl => new Level
            {
                Name = pl.DEA_KDWS_HRlevel.navn,
                LevelNumber = pl.DEA_KDWS_HRlevel.niveau
            }).ToList();
        }

        /// <summary>
        /// Gets the List  of ProductReview objects for the review text related to Hr Product.
        /// </summary>
        /// <param name="hRproduct"></param>
        /// <returns></returns>
        private static List<ProductReview> GetReviews(this DEA_KDWS_HRproduct hRproduct)
        {
            var reviews = hRproduct.DEA_KDWS_HRProductReviews.ToList();

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
        /// Get CoreData BundleProduct
        /// </summary>
        /// <param name="kdBundleProducts"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<BundleProduct> ToCoreDataBundleProduct(this ICollection<DEA_KDWS_HRBundle_Products> kdBundleProducts,
           Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var bundleProducts = kdBundleProducts?.Select(x => new BundleProduct
            {
                DiscountPercentage = x.discount_percentage ?? 0m,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(x.DEA_KDWS_HRproduct.medietype.ToLower()),
                Isbn = x.DEA_KDWS_HRproduct.ISBN13,
                Title = x.DEA_KDWS_HRproduct.titel,
                SalesConfiguration = x.DEA_KDWS_HRproduct.GetBundleProductSaleConfiguration(shopServicesApiClient, configManager),
                InStock = x.DEA_KDWS_HRproduct.lagertal != null && x.DEA_KDWS_HRproduct.lagertal > 0,
                IsPublished = x.DEA_KDWS_HRproduct.udgivelsesdato <= DateTime.Now,
                ProductPrices = null,
                MediaType = new MediaType
                {
                    Name = x.DEA_KDWS_HRproduct.medietype,
                    WebShop = WebShop.HansReitzel
                },
                MaterialType = new MaterialType
                {
                    Name = x.DEA_KDWS_HRproduct.materialetype,
                    WebShop = WebShop.HansReitzel
                },
                IsNextPrintRunPlanned = x.DEA_KDWS_HRproduct.IsNextPrintRunPlanned,
                CurrentPrintRunPublishDate = x.DEA_KDWS_HRproduct.udgivelsesdato,
                PublishDate = x.DEA_KDWS_HRproduct.FirstPrintPublishDate,
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
        /// <param name="hrProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static SalesConfiguration GetBundleProductSaleConfiguration(this DEA_KDWS_HRproduct hrProduct,
           Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var isPhysical = configManager.PhysicalMediaTypes.Contains(hrProduct.medietype.ToLower());
            if (isPhysical)
            {
                return new SalesConfiguration().SetPhysicalBundleProductSaleConfiguration(hrProduct.ISBN13, hrProduct.pris_med_moms.GetValueOrDefault(0),
                    hrProduct.pris_uden_moms.GetValueOrDefault(0), hrProduct.LastUpdated);
            }
            else
            {
                return shopServicesApiClient.SalesConfiguration.GetSalesConfiguration(hrProduct.ISBN13, WebShop.HansReitzel);
            }
        }

        /// <summary>
        /// Get MembershipPath
        /// </summary>
        /// <param name="kdMembershipPaths"></param>
        /// <returns></returns>
        private static List<string> GetMembershipPaths(this ICollection<DEA_KDWS_HRBundleMembership> kdMembershipPaths)
        {
            return kdMembershipPaths?
                .Select(x => x.MembershipPath)
                .ToList();
        }

        /// <summary>
        /// Get Levels of Products
        /// </summary>
        /// <param name="hrProducts"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this ICollection<DEA_KDWS_HRBundle_Products> hrProducts)
        {
            if (hrProducts == null || hrProducts.Any() == false)
                return null;

            var levels = new List<Level>();
            foreach (var hrProduct in hrProducts)
            {
                levels.AddRange(hrProduct.DEA_KDWS_HRproduct.GetLevels());
            }

            return levels;
        }

        /// <summary>
        /// Get Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Area> GetAreas(this DEA_KDWS_HRcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(4);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Area { Id = x.Item1, Name = x.Item2, WebShop = WebShop.HansReitzel }).ToList();
            }

            return new Area[] { }.ToList();
        }

        /// <summary>
        /// Get Subjects of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Subject> GetSubjects(this DEA_KDWS_HRcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(5);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Subject { Id = x.Item1, Name = x.Item2, AreaId = x.Item3, WebShop = WebShop.HansReitzel }).ToList();
            }

            return new Subject[] { }.ToList();
        }

        /// <summary>
        /// Get Sub Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<SubArea> GetSubAreas(this DEA_KDWS_HRcategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(6);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new SubArea
                {
                    Id = x.Item1,
                    Name = x.Item2,
                    WebShop = WebShop.HansReitzel,
                    SubjectId = x.Item3.GetValueOrDefault(0)
                }).ToList();
            }

            return new SubArea[] { }.ToList();
        }

        /// <summary>
        /// Get Series of Product and return list of series
        /// </summary>
        /// <param name="hrProduct"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<Series> GetSeries(this DEA_KDWS_HRproduct hrProduct, IConfigurationManager configManager)
        {
            return hrProduct.DEA_KDWS_HRproductseries.Select(x => x.DEA_KDWS_HRseries)
                .Select(x => new Series
                {
                    Id = x.id,
                    Name = x.navn,
                    WebShop = WebShop.HansReitzel,
                    ImageUrl = Common.ModelsMapping.GetSystemSeriesImageUrl(x.id, configManager, x.Is_Image_Uploaded),
                    LastUpdated = x.LastUpdated,
                    IsSystemSeries = x.Type == 1,
                    ParentSerieId = x.parent_id
                }).ToList();
        }

        /// <summary>
        /// Get Product Contributors
        /// </summary>
        /// <param name="hrProduct"></param>
        /// <returns></returns>
        private static List<Contributor> GetContributors(this DEA_KDWS_HRproduct hrProduct)
        {
            return hrProduct.DEA_KDWS_HRproductcontributors.OrderBy(c => c.firmpers_stamdata_sortorder).Select(c => new Contributor
            {
                Id = c.DEA_KDWS_HRContributors.contributor_id,
                AuthorNumber = c.DEA_KDWS_HRContributors.Forfatternr,
                FirstName = c.DEA_KDWS_HRContributors.contributor_fornavn,
                LastName = c.DEA_KDWS_HRContributors.contributor_efternavn,
                Photo = c.DEA_KDWS_HRContributors.contributor_foto,
                Url = c.DEA_KDWS_HRContributors.contributor_profileLink,
                ContibutorType = (ContributorType)c.role_id
            }).ToList();
        }

        /// <summary>
        /// Extracts and returns a Tuple of int (CategoryId), string (Category Name), and int? (Parent category Id), from the passed in list of category objects and the given level.
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static List<Tuple<int, string, int?>> GetCategoriesByLevel(this DEA_KDWS_HRcategory[] prodCategories, int level)
        {
            var categories =
                prodCategories.Where(x => x.niveau == level)
                .Select(x => new Tuple<int, string, int?>(x.id, x.navn.Trim(), x.parent))
                .ToList();

            return categories;
        }

        public static ContributorDetails ToCoreDataContributor(this DEA_KDWS_HRContributors kdContributors)
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
                ContibutorType = kdContributors.DEA_KDWS_HRproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_HRproductcontributors.Select(x => x.DEA_KDWS_HRproduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography = kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.HansReitzel }
            };

            return contributor;
        }

        public static ContributorDetailsV2 ToCoreDataContributorV2(this DEA_KDWS_HRContributors kdContributors, List<ProfileImage> photos)
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
                ContibutorType = kdContributors.DEA_KDWS_HRproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_HRproductcontributors.Select(x => x.DEA_KDWS_HRproduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography = kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.HansReitzel }
            };

            return contributor;
        }

        public static Contributor ToCoreDataContributor(this DEA_KDWS_HRContributors kdContributors, ContributorType type)
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
                    string.Join(", ", kdContributors.DEA_KDWS_HRproductcontributors.Select(x => x.DEA_KDWS_HRproduct)
                        .Select(x => x.titel)
                        .Take(3)),
            };

            return contributor;
        }
    }
}