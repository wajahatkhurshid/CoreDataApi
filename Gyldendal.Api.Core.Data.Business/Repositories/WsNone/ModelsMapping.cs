using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using System.Collections.Generic;
using MediaType = Gyldendal.Api.CoreData.Contracts.Models.MediaType;

namespace Gyldendal.Api.CoreData.Business.Repositories.WsNone
{
    public static class ModelsMapping
    {
        /// <summary>
        /// Creates Work Object Using DEA_KDWS_GUproduct
        /// </summary>
        /// <param name="ekeyProduct"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configurationManager"></param>
        /// <returns></returns>
        internal static Work ToCoreDataWork(this DEA_KDWS_Ekeyproduct ekeyProduct, ICoverImageUtil imageUtil, IConfigurationManager configurationManager)
        {
            var product = ekeyProduct.ToCoreDataProduct(imageUtil, configurationManager);

            var work = new Work
            {
                Id = ekeyProduct.work_id.GetValueOrDefault(0),
                Title = ekeyProduct.titel,
                WebShop = WebShop.None,
                Products = new List<Product> { product },
            };

            return work;
        }

        /// <summary>
        /// Creates Product Object using DEA_KDWS_GUproduct Object
        /// </summary>
        /// <param name="ekeyProduct"></param>
        /// <param name="imageUtil"></param>
        /// <param name="configurationManager"></param>
        /// <returns></returns>
        private static Product ToCoreDataProduct(this DEA_KDWS_Ekeyproduct ekeyProduct, ICoverImageUtil imageUtil, IConfigurationManager configurationManager)
        {
            var product = new Product
            {
                Id = ekeyProduct.vare_id,
                Isbn13 = ekeyProduct.vare_id,
                Title = ekeyProduct.titel,
                Subtitle = ekeyProduct.undertitel,
                Description = ekeyProduct.langbeskrivelse.RepairHtml(),
                MediaType = new MediaType { Name = ekeyProduct.medietype, WebShop = WebShop.None },
                WorkId = ekeyProduct.work_id,
                PublishDate = ekeyProduct.FirstPrintPublishDate,
                CurrentPrintRunPublishDate = ekeyProduct.udgivelsesdato,
                IsNextPrintPlanned = ekeyProduct.IsNextPrintRunPlanned,
                //Url = vare.webadresse,
                ProductUrls = Common.ModelsMapping.GetProductUrls(
                    new InternalObjects.ProductUrlInput
                    {
                        webShop = WebShop.None,
                        productId = ekeyProduct.vare_id,
                        mediaType = ekeyProduct.medietype,
                        isPhysical = false,
                        url = ekeyProduct.Website,
                        hasAttachments = true,
                        configManager = configurationManager
                    }),
                OriginalCoverImageUrl = imageUtil.GetOriginalImageUrl(ekeyProduct.vare_id),
                ProductType = ProductType.SingleProduct,
                WebShop = WebShop.None,
                ProductSource = ProductSource.Rap,
                Imprint = ekeyProduct.Imprint
            };

            return product;
        }
    }
}