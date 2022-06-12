using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Policy;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.Utils;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Models.Bundle;
using PorterApi = Gyldendal.Api.CoreData.Services.PorterApiClient;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Porter.Mapping
{
    public static class PorterWorkModelsMapping
    {
        public static Work ToCoreDataWork(this CoreData.Services.PorterApiClient.Work work, ICoverImageUtil imageUtil, IConfigurationManager configurationManager, ShopServices.ApiClient.Client shopServicesApiClient, WebShop webshop)
        {
            var areasList = new List<Area>();
            var subAreasList = new List<SubArea>();
            var subjectsList = new List<Subject>();
            var levelsList = new List<Level>();
            foreach (var product in work.Products)
            {
                
                    areasList.AddRange(product.Areas.Select(area => new Area() { Id =area.Id, Name = area.Name, WebShop = webshop }));

                    levelsList.AddRange(product.ProductEducationSubjectLevels.Select(level => new Level() { LevelNumber = level.LevelNumber, Name = level.Name }));

                    subjectsList.AddRange(product.Subjects.Select(su => su.ToCoreDataSubjects(webshop)));

                    subAreasList.AddRange(product.SubAreas.Select(sa => sa.ToCoreDataSubArea(webshop)));
            }
            
            return new Work()
            {
                Areas = areasList,
                Description = work.Description,
                Title = work.Title,
                Levels = levelsList,
                Products = work.Products.Select(p =>
                    p.ToCoreDataProduct(shopServicesApiClient, imageUtil, configurationManager, webshop)).ToList(),
                WebShop = webshop,
                Id = work.Id,
                SubAreas = subAreasList,
                Subjects = subjectsList,
               ThemaCodes = null
            };
        }

        public static Work ToCoreDataWork(this varer vare, List<PorterApi.Product> porterProducts,
            IEnumerable<portersitekategorier> siteCategories, ShopServices.ApiClient.Client shopServicesApiClient,
            ICoverImageUtil imageUtil, IConfigurationManager configurationManager, WebShop webShop)
        {
            bool subBundleProductsWithDifferentWorkIds = false;

            if (porterProducts.Count > 0)
                subBundleProductsWithDifferentWorkIds = porterProducts.Select(pp => pp.WorkId).Distinct().Count() > 1;
            
            var coreDataProducts = porterProducts.Select(p =>
                p.ToCoreDataProduct(shopServicesApiClient, imageUtil, configurationManager, webShop)).ToList();
            
            var bundleProducts = coreDataProducts.Select(x => x.ToCoreDataBundleProduct()).ToList();
            
            Product packageProduct = new Product()
            {
                Id = vare.id,
                Isbn13 = vare.id,
                Title = vare.titel,
                Subtitle = vare.undertitel,
                Description = vare.langbeskrivelse.RepairHtml(),
                MediaType = new MediaType()
                {
                    Name = vare.medietype,
                    WebShop = webShop
                },
                MaterialType = new MaterialType()
                {
                    Name = "", //TODO ?
                    WebShop = webShop

                },
                PublishDate = vare.kd_gyldig_fra,
                WorkId = Convert.ToInt32(vare.id.ToLower().Replace("pak_", "")),
                SampleUrl = null, // null for bundle product
                SeoText = vare.SEOdescription,
                Edition = vare.udgave,
                Pages = !string.IsNullOrEmpty(vare.sideantal) ? Convert.ToInt32(vare.sideantal) : 0,
                ExcuseCode = vare.undskyldningskode,
                Publisher = vare.forlag,
                DurationInMinutes = vare.spilletid.ToInt(),
                InStock = bundleProducts.All(x => !x.IsPhysical || (x.IsPhysical && x.InStock)),
                IsPublished = vare.udgivelsesdato <= DateTime.Now,
                IsNextPrintPlanned = false,
                OriginalCoverImageUrl = vare.illustrationURL,
                IsPhysical = bundleProducts.All(x => x.IsPhysical),
                ProductUrls = null,
                //LastUpdated = vare.LastUpdated, //from campaign table
                WebShop = webShop,
                ProductType = ProductType.Bundle,
                BundleProducts = bundleProducts,
                //MembershipPaths = guBundle.DEA_KDWS_GUBundleMembership.GetMembershipPaths(), //from campaign Membership Table
                MediaTypeRank = MediaTypeRank.GetMediaTypeRank(vare.medietype),
                MaterialTypeRank = 1, // using default value for now
                Distributors = null, // Might be filled in future.
                Series = null,
                Contributors = null,
                Reviews = null,
                CoverImages = null
            };

            var bundle = new List<Product>();
            bundle.Add(packageProduct);

            if (!subBundleProductsWithDifferentWorkIds)
                return UpdateWorkFromSiteCategories(bundle, siteCategories, webShop, porterProducts);
            else
                return UpdateWorkFromPorterProducts(bundle, porterProducts, webShop);
        }

        /// <summary>
        /// Update Area,SubArea,Level,Subject on work from Porter Products
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="porterProducts"></param>
        /// <param name="webShop"></param>
        /// <returns></returns>
        private static Work UpdateWorkFromPorterProducts(List<Product> bundle, IEnumerable<PorterApi.Product> porterProducts, WebShop webShop)
        {
            var areasList = new List<Area>();
            var subAreasList = new List<SubArea>();
            var subjectsList = new List<Subject>();
            var levelsList = new List<Level>();
            foreach (var product in porterProducts)
            {
                
                areasList.AddRange(product.Areas.Select(area => new Area() { Id = area.Id, Name = area.Name, WebShop = webShop }));

                subAreasList.AddRange(product.SubAreas.Select(subarea => new SubArea() { Id = subarea.Id, Name = subarea.Name, WebShop = webShop, SubjectId = subarea.SubjectId}));

                levelsList.AddRange(product.ProductEducationSubjectLevels.Select(level => new Level() { LevelNumber = level.LevelNumber, Name = level.Name }));

                subjectsList.AddRange(product.Subjects.Select(subject => new Subject() { Id = subject.Id, WebShop = webShop, AreaId = subject.AreaId, Name = subject.Name }));
                
            }

            return new Work()
            {
                Id = bundle[0].WorkId.HasValue ? bundle[0].WorkId.Value * -1:0,
                Description = bundle[0].Description,
                Title = bundle[0].Title,
                Products = bundle,
                WebShop = webShop,
                Areas = areasList,
                SubAreas = subAreasList,
                Levels = levelsList,
                Subjects = subjectsList,
            };
        }

        /// <summary>
        /// Update Area,SubArea,Level,Subject on work from Site Categories table in KD
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="siteCategories"></param>
        /// <param name="webShop"></param>
        /// <returns></returns>
        private static Work UpdateWorkFromSiteCategories(List<Product> bundle, IEnumerable<portersitekategorier> siteCategories, WebShop webShop, IEnumerable<PorterApi.Product> porterProducts)
        {
            var levelsList = new List<Level>();
            foreach (var product in porterProducts)
            {
                levelsList.AddRange(product.ProductEducationSubjectLevels.Select(level => new Level() { LevelNumber = level.LevelNumber, WebShop = webShop, AreaId = level.AreaId, Name = level.Name }));
            }
            return new Work()
            {

                Id = bundle[0].WorkId.HasValue ? bundle[0].WorkId.Value : 0,
                Description = bundle[0].Description,
                Title = bundle[0].Title,
                Products = bundle,
                WebShop = webShop,
                Areas = siteCategories.Select(a => new Area()
                {
                    Id = 0,
                    Name = a.area,
                    WebShop = webShop
                }).ToList(),
                SubAreas = siteCategories.Select(sa => new SubArea()
                {
                    Id = 0,
                    Name = sa.subarea,
                    SubjectId = 0,
                    WebShop = webShop
                }).ToList(),
                Subjects = siteCategories.Select(sa => new Subject()
                {
                    Id = 0,
                    Name = sa.subarea,
                    AreaId = 0,
                    WebShop = webShop
                }).ToList(),

                Levels = levelsList
            };
        }

    }
}
