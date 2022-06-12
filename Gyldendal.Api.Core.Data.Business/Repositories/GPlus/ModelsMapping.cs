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
using Gyldendal.Api.ShopServices.Contracts.SalesConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using MaterialType = Gyldendal.Api.CoreData.Contracts.Models.MaterialType;
using MediaType = Gyldendal.Api.CoreData.Contracts.Models.MediaType;

namespace Gyldendal.Api.CoreData.Business.Repositories.GPlus
{
    public static class ModelsMapping
    {
        /// <summary>
        /// Creates Work Object Using DEA_KDWS_GPlusproduct
        /// </summary>
        /// <param name="gPlusProduct"></param>
        /// <param name="prodCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="webShop"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Work ToCoreDataWork(this DEA_KDWS_GPlusproduct gPlusProduct, DEA_KDWS_GPluscategory[] prodCategories, ShopServices.ApiClient.Client shopServicesApiClient, ICoverImageUtil imageUtil, IConfigurationManager configManager, WebShop webShop, bool isSupplementaryMaterial = false)
        {
            var product = gPlusProduct.ToCoreDataProduct(imageUtil, configManager, isSupplementaryMaterial);

            var work = new Work
            {
                Id = gPlusProduct.DEA_KDWS_GPluswork.work_id,
                Title = gPlusProduct.DEA_KDWS_GPluswork.titel,
                WebShop = WebShop.ClubBogklub,
                Levels = gPlusProduct.GetLevels(),
                Areas = prodCategories.GetAreas(),
                Subjects = prodCategories.GetSubjects(),
                SubAreas = prodCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null, // Might be filled in future.
                ThemaCodes = gPlusProduct.GetThemaCodes()
            };

            return work;
        }

        internal static WorkReview ToCoreDataWorkReview(this Gyldendal.PulsenServices.Api.Contracts.Product.ProductReview productReview, int workId, WebShop webShop)
        {
            if (productReview == null)
            {
                throw new ArgumentException("Argument workReview cannot be null.");
            }

            return new WorkReview
            {
                WorkReviewId = productReview.ReviewId,
                Review = productReview.Review,
                WorkId = workId,
                Rating = productReview.Rating,
                Priority = productReview.Priority,
                AuthorInfo = productReview.Source,
                Id = $"{productReview.ReviewId}_{webShop:D}",
                WebShopId = webShop
            };
        }

        private static List<ThemaCode> GetThemaCodes(this DEA_KDWS_GPlusproduct gPlusproduct)
        {
            return gPlusproduct?.DEA_KDWS_GPlusProductThemacode.Select(x => new ThemaCode
            {
                Code = x.DEA_KDWS_GPlusThemacode.ThemaCode,
                Description = x.DEA_KDWS_GPlusThemacode.DanishDescription,
                Id = x.ThemaCodeId
            }).ToList();
        }

        /// <summary>
        /// Creates Work Object using DEA_KDWS_GPlusBundle Object
        /// </summary>
        /// <param name="gPlusBundle"></param>
        /// <param name="gPlusCategories"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Work ToCoreDataWork(this DEA_KDWS_GPlusBundle gPlusBundle, DEA_KDWS_GPluscategory[] gPlusCategories, ShopServices.ApiClient.Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configManager)
        {
            var product = gPlusBundle.ToCoreDataProduct(shopServicesApiClient, configManager);

            var work = new Work
            {
                Id = gPlusBundle.work_id ?? 0,
                Title = gPlusBundle.titel,
                WebShop = WebShop.ClubBogklub,
                Levels = gPlusBundle.DEA_KDWS_GPlusBundle_Products.GetLevels(),
                Areas = gPlusCategories.GetAreas(),
                Subjects = gPlusCategories.GetSubjects(),
                SubAreas = gPlusCategories.GetSubAreas(),
                Products = new List<Product> { product },

                Description = null,
                ThemaCodes = null
            };

            return work;
        }

        internal static void PopulateThemaCodeRelatedFields(this Product product, DEA_KDWS_GPlusproduct gPlusProduct)
        {
            if (!(gPlusProduct.DEA_KDWS_GPlusProductThemacode?.Any() ?? false))
            {
                return;
            }

            var themaCodes = gPlusProduct.DEA_KDWS_GPlusProductThemacode.Select(x => x.DEA_KDWS_GPlusThemacode).ToList();
            var genres = themaCodes.Where(x => x.ThemaCode.Length == 2);
            var categories = themaCodes.Where(x => x.ThemaCode.Length == 3);
            var subCategories = themaCodes.Where(x => x.ThemaCode.Length == 4);
            product.Genres = genres.Select(g => new Genre
            {
                Code = g.ThemaCode,
                Name = g.DanishDescription
            }).ToList();
            product.Categories = categories.Select(g => new Category
            {
                Code = g.ThemaCode,
                Name = g.DanishDescription
            }).ToList();
            product.SubCategories = subCategories.Select(g => new SubCategory
            {
                Code = g.ThemaCode,
                Name = g.DanishDescription
            }).ToList();
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_GPlusproduct Object
        /// </summary>
        /// <param name="gPlusProduct"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configManager"></param>
        /// <param name="isSupplementaryMaterial"></param>
        /// <returns></returns>
        internal static Product ToCoreDataProduct(this DEA_KDWS_GPlusproduct gPlusProduct, ICoverImageUtil imageUtil, IConfigurationManager configManager, bool isSupplementaryMaterial)
        {
            var variantImage = imageUtil.GetProductImagesVariant(gPlusProduct.ISBN13, DataScope.GyldendalPlus);
            var hasAttachments = gPlusProduct.DEA_KDWS_GPlusattachments.Any(x => x.is_secured == true && x.kd_slettet == 0);

            var product = new Product
            {
                Id = gPlusProduct.vare_id,
                Isbn13 = gPlusProduct.ISBN13,
                Title = gPlusProduct.titel,
                Subtitle = gPlusProduct.undertitel,
                Description = gPlusProduct.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = gPlusProduct.materialetype, WebShop = WebShop.ClubBogklub },
                MediaType = new MediaType { Name = gPlusProduct.medietype, WebShop = WebShop.ClubBogklub },
                WorkId = gPlusProduct.work_id,
                PublishDate = gPlusProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = gPlusProduct.udgivelsesdato,
                SampleUrl = gPlusProduct.ReadingSamples,
                SeoText = gPlusProduct.SEO_Text,
                Edition = gPlusProduct.udgave,
                Pages = gPlusProduct.sider,
                ExcuseCode = gPlusProduct.undskyldningskode,
                Publisher = gPlusProduct.forlag,
                DurationInMinutes = gPlusProduct.spilletid.ToInt(),
                InStock = gPlusProduct.lagertal != null && gPlusProduct.lagertal > 0,
                Series = gPlusProduct.GetSeries(configManager),
                Contributors = gPlusProduct.GetContributors(),
                IsPublished = gPlusProduct.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = gPlusProduct.IsNextPrintRunPlanned,
                CoverImages = variantImage?.ProductImages,
                OriginalCoverImageUrl = variantImage?.OriginalCoverImageUrl,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(gPlusProduct.medietype.ToLower()),
                ProductUrls = GetProductUrls(gPlusProduct.vare_id, gPlusProduct.medietype, gPlusProduct.Url, hasAttachments, configManager),
                LastUpdated = gPlusProduct.LastUpdated,
                WebShop = WebShop.ClubBogklub,
                Reviews = gPlusProduct.GetReviews(),
                ProductType = ProductType.SingleProduct,
                IsSupplementaryMaterial = isSupplementaryMaterial,
                MaterialTypeRank = gPlusProduct.materialetypeRank.GetValueOrDefault(1),
                MediaTypeRank = gPlusProduct.medietypeRank.GetValueOrDefault(1),
                GrossWeight = gPlusProduct.Gross_weight.GetValueOrDefault(),
                NetWeight = gPlusProduct.Net_weight.GetValueOrDefault(),
                Height = gPlusProduct.Height.GetValueOrDefault(),
                Width = gPlusProduct.Width.GetValueOrDefault(),
                ThicknessDepth = gPlusProduct.Thickness_depth.GetValueOrDefault(),
                Distributors = null, // Might be filled in future.
                InspectionCopyAllowed = gPlusProduct.gennemsynseksemplar.GetValueOrDefault(0) > 0,
                ProductSource = ProductSource.Rap,
                Imprint = gPlusProduct.Imprint
            };

            // Set the Last updated date to Maximum of Product last updated date or Sales configuration Created date
            if (product.SalesConfiguration != null && product.SalesConfiguration.CreatedDate > product.LastUpdated)
            {
                product.LastUpdated = product.SalesConfiguration.CreatedDate;
            }

            // Currently we set the IsBuyable bit true as we have no specific requirement.
            product.IsBuyable = true;

            product.PopulateThemaCodeRelatedFields(gPlusProduct);

            return product;
        }

        private static List<ProductUrl> GetProductUrls(string productId, string mediaType, string url, bool hasAttachments, IConfigurationManager configManager)
        {
            return Common.ModelsMapping.GetProductUrls(
                new InternalObjects.ProductUrlInput
                {
                    webShop = WebShop.ClubBogklub,
                    productId = productId,
                    mediaType = mediaType,
                    isPhysical = configManager.PhysicalMediaTypes.Contains(mediaType.ToLower()),
                    url = url,
                    hasAttachments = hasAttachments,
                    configManager = configManager
                });
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_GPlusBundle Object
        /// </summary>
        /// <param name="gPlusBundle"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        public static Product ToCoreDataProduct(this DEA_KDWS_GPlusBundle gPlusBundle, ShopServices.ApiClient.Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var bundleProducts = ToCoreDataBundleProduct(gPlusBundle.DEA_KDWS_GPlusBundle_Products, shopServicesApiClient, configManager);

            var product = new Product
            {
                Id = gPlusBundle.bundle_id,
                Isbn13 = gPlusBundle.ISBN13,
                Title = gPlusBundle.titel,
                Subtitle = gPlusBundle.undertitel,
                Description = gPlusBundle.langbeskrivelse.RepairHtml(),
                MaterialType = new MaterialType { Name = gPlusBundle.materialetype, WebShop = WebShop.ClubBogklub },
                MediaType = new MediaType { Name = gPlusBundle.medietype, WebShop = WebShop.ClubBogklub },
                WorkId = gPlusBundle.work_id,
                PublishDate = gPlusBundle.udgivelsesdato,
                SampleUrl = gPlusBundle.Url,
                SeoText = gPlusBundle.SEO_Text,
                Edition = gPlusBundle.udgave,
                Pages = gPlusBundle.sider,
                ExcuseCode = gPlusBundle.undskyldningskode,
                Publisher = gPlusBundle.forlag,
                DurationInMinutes = gPlusBundle.spilletid.ToInt(),
                InStock = bundleProducts.All(x => !x.IsPhysical || (x.IsPhysical && x.InStock)),
                IsBuyable = bundleProducts.All(x => x.IsBuyable),
                IsPublished = gPlusBundle.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = false,
                OriginalCoverImageUrl = gPlusBundle.illustrationURL,
                IsPhysical = bundleProducts.All(x => x.IsPhysical),
                
                // Bundle are considered as physical and with no attachments
                ProductUrls = GetProductUrls(gPlusBundle.bundle_id, gPlusBundle.medietype, gPlusBundle.Url, hasAttachments:false, configManager),
                
                LastUpdated = gPlusBundle.LastUpdated,
                WebShop = WebShop.ClubBogklub,
                ProductType = ProductType.Bundle,
                BundleProducts = bundleProducts,
                MembershipPaths = gPlusBundle.DEA_KDWS_GPlusBundleMembership.GetMembershipPaths(),
                MediaTypeRank = gPlusBundle.materialetypeRank.GetValueOrDefault(1),
                MaterialTypeRank = gPlusBundle.materialetypeRank.GetValueOrDefault(1),

                Distributors = null, // Might be filled in future.
                Series = null,
                Contributors = null,
                Reviews = null,
                CoverImages = null
            };
            return product;
        }

        public static ContributorDetails ToCoreDataContributor(this DEA_KDWS_GPlusContributors kdContributors)
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
                ContibutorType = kdContributors.DEA_KDWS_GPlusproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_GPlusproductcontributors.Select(x => x.DEA_KDWS_GPlusproduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography =
                    kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.ClubBogklub }
            };

            return contributor;
        }

        public static ContributorDetailsV2 ToCoreDataContributorV2(this DEA_KDWS_GPlusContributors kdContributors, List<ProfileImage> photos)
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
                ContibutorType = kdContributors.DEA_KDWS_GPlusproductcontributors
                    .Select(x => (ContributorType)x.role_id).Distinct().ToList(),
                Bibliography = string.Join(", ", kdContributors.DEA_KDWS_GPlusproductcontributors.Select(x => x.DEA_KDWS_GPlusproduct)
                    .Select(x => x.titel)
                    .Take(3)),
                Biography =
                    kdContributors.contributor_langbeskrivelse,
                WebShopsId = new List<WebShop> { WebShop.ClubBogklub }
            };

            return contributor;
        }

        public static Contributor ToCoreDataContributor(this DEA_KDWS_GPlusContributors kdContributors, ContributorType type)
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
                    string.Join(", ", kdContributors.DEA_KDWS_GPlusproductcontributors.Select(x => x.DEA_KDWS_GPlusproduct)
                        .Select(x => x.titel)
                        .Take(3)),
            };

            return contributor;
        }

        /// <summary>
        /// Get CoreData BundleProduct
        /// </summary>
        /// <param name="kdBundleProducts"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<BundleProduct> ToCoreDataBundleProduct(this ICollection<DEA_KDWS_GPlusBundle_Products> kdBundleProducts, ShopServices.ApiClient.Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var bundleProducts = kdBundleProducts?.Select(x => new BundleProduct
            {
                DiscountPercentage = x.discount_percentage ?? 0m,
                IsPhysical = configManager.PhysicalMediaTypes.Contains(x.DEA_KDWS_GPlusproduct.medietype.ToLower()),
                Isbn = x.DEA_KDWS_GPlusproduct.ISBN13,
                Title = x.DEA_KDWS_GPlusproduct.titel,
                SalesConfiguration = x.DEA_KDWS_GPlusproduct.GetBundleProductSaleConfiguration(shopServicesApiClient, configManager),
                InStock = x.DEA_KDWS_GPlusproduct.lagertal != null && x.DEA_KDWS_GPlusproduct.lagertal > 0,
                IsPublished = x.DEA_KDWS_GPlusproduct.udgivelsesdato <= DateTime.Now,

                // Currently we set the IsBuyable bit true as we have no specific requirement.
                IsBuyable = true,
                MediaType = new MediaType
                {
                    Name = x.DEA_KDWS_GPlusproduct.medietype,
                    WebShop = WebShop.ClubBogklub
                },
                MaterialType = new MaterialType
                {
                    Name = x.DEA_KDWS_GPlusproduct.materialetype,
                    WebShop = WebShop.ClubBogklub
                },
                ProductPrices = null,
                IsNextPrintRunPlanned = x.DEA_KDWS_GPlusproduct.IsNextPrintRunPlanned,
                CurrentPrintRunPublishDate = x.DEA_KDWS_GPlusproduct.udgivelsesdato,
                PublishDate = x.DEA_KDWS_GPlusproduct.FirstPrintPublishDate,
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
        /// <param name="gPlusProduct"></param>
        /// <param name="shopServicesApiClient"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static SalesConfiguration GetBundleProductSaleConfiguration(this DEA_KDWS_GPlusproduct gPlusProduct,
            ShopServices.ApiClient.Client shopServicesApiClient, IConfigurationManager configManager)
        {
            var isPhysical = configManager.PhysicalMediaTypes.Contains(gPlusProduct.medietype.ToLower());
            if (isPhysical)
            {
                return new SalesConfiguration().SetPhysicalBundleProductSaleConfiguration(gPlusProduct.ISBN13, gPlusProduct.pris_med_moms.GetValueOrDefault(0),
                    gPlusProduct.pris_uden_moms.GetValueOrDefault(0), gPlusProduct.LastUpdated);
            }
            else
            {
                return shopServicesApiClient.SalesConfiguration.GetSalesConfiguration(gPlusProduct.ISBN13, WebShop.ClubBogklub);
            }
        }

        /// <summary>
        /// Get MembershipPath
        /// </summary>
        /// <param name="kdMembershipPaths"></param>
        /// <returns></returns>
        private static List<string> GetMembershipPaths(this ICollection<DEA_KDWS_GPlusBundleMembership> kdMembershipPaths)
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
        private static List<SubArea> GetSubAreas(this DEA_KDWS_GPluscategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(3);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new SubArea
                {
                    Id = x.Item1,
                    Name = x.Item2,
                    WebShop = WebShop.ClubBogklub,
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
        private static List<Subject> GetSubjects(this DEA_KDWS_GPluscategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(2);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Subject { Id = x.Item1, Name = x.Item2, AreaId = x.Item3, WebShop = WebShop.ClubBogklub, }).ToList();
            }

            return new Subject[] { }.ToList();
        }

        /// <summary>
        /// Get Areas of Product
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <returns></returns>
        private static List<Area> GetAreas(this DEA_KDWS_GPluscategory[] prodCategories)
        {
            var categories = prodCategories.GetCategoriesByLevel(1);

            if (categories != null && categories.Any())
            {
                return categories.Select(x => new Area { Id = x.Item1, Name = x.Item2, WebShop = WebShop.ClubBogklub }).ToList();
            }

            return new Area[] { }.ToList();
        }

        /// <summary>
        /// Get Levels of Product
        /// </summary>
        /// <param name="gPlusProduct"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this DEA_KDWS_GPlusproduct gPlusProduct)
        {
            return gPlusProduct.DEA_KDWS_GPlusProductLevels.Select(pl => new Level
            {
                Name = pl.DEA_KDWS_GPluslevel.navn,
                LevelNumber = pl.DEA_KDWS_GPluslevel.niveau
            }).ToList();
        }

        /// <summary>
        /// Get Levels of Products
        /// </summary>
        /// <param name="gPlusProducts"></param>
        /// <returns></returns>
        private static List<Level> GetLevels(this ICollection<DEA_KDWS_GPlusBundle_Products> gPlusProducts)
        {
            if (gPlusProducts == null || gPlusProducts.Any() == false)
            {
                return null;
            }

            var levels = new List<Level>();
            foreach (var gPlusProduct in gPlusProducts)
            {
                levels.AddRange(gPlusProduct.DEA_KDWS_GPlusproduct.GetLevels());
            }

            return levels;
        }

        /// <summary>
        /// Get Series of Product and return list of series
        /// </summary>
        /// <param name="gPlusProduct"></param>
        /// <param name="configManager"></param>
        /// <returns></returns>
        private static List<Series> GetSeries(this DEA_KDWS_GPlusproduct gPlusProduct, IConfigurationManager configManager)
        {
            return gPlusProduct.DEA_KDWS_GPlusproductseries.Select(x => x.DEA_KDWS_GPlusseries)
                .Select(x => new Series
                {
                    Id = x.id,
                    Name = x.navn,
                    WebShop = WebShop.ClubBogklub,
                    LastUpdated = x.LastUpdated,
                    ImageUrl = Common.ModelsMapping.GetSystemSeriesImageUrl(x.id, configManager, x.Is_Image_Uploaded),
                    IsSystemSeries = x.Type == 1,
                    ParentSerieId = x.parent_id
                }).ToList();
        }

        /// <summary>
        /// Gets the List  of ProductReview objects for the review text related to gPlus Product.
        /// </summary>
        /// <param name="gPlusProduct"></param>
        /// <returns></returns>
        private static List<ProductReview> GetReviews(this DEA_KDWS_GPlusproduct gPlusProduct)
        {
            var reviews = gPlusProduct.DEA_KDWS_GPlusProductReviews.ToList();

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
        /// <param name="gPlusProduct"></param>
        /// <returns></returns>
        private static List<Contributor> GetContributors(this DEA_KDWS_GPlusproduct gPlusProduct)
        {
            return gPlusProduct.DEA_KDWS_GPlusproductcontributors.OrderBy(c => c.firmpers_stamdata_sortorder).Select(c => new Contributor
            {
                Id = c.DEA_KDWS_GPlusContributors.contributor_id,
                AuthorNumber = c.DEA_KDWS_GPlusContributors.Forfatternr,
                FirstName = c.DEA_KDWS_GPlusContributors.contributor_fornavn,
                LastName = c.DEA_KDWS_GPlusContributors.contributor_efternavn,
                Photo = c.DEA_KDWS_GPlusContributors.contributor_foto,
                Url = c.DEA_KDWS_GPlusContributors.contributor_profileLink,
                ContibutorType = (ContributorType)c.role_id
            }).ToList();
        }

        /// <summary>
        /// Extracts and returns a Tuple of int (CategoryId), string (Category Name), and int? (Parent category Id), from the passed in list of category objects and the given level.
        /// </summary>
        /// <param name="prodCategories"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private static List<Tuple<int, string, int?>> GetCategoriesByLevel(this DEA_KDWS_GPluscategory[] prodCategories, int level)
        {
            var categories =
                prodCategories.Where(x => x.niveau == level)
                .Select(x => new Tuple<int, string, int?>(x.id, x.navn.Trim(), x.parent))
                .ToList();

            return categories;
        }
    }
}