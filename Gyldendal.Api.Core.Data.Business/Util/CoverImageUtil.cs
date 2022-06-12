using Gyldendal.Api.CoreData.Business.InternalObjects;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MMSImageService;

namespace Gyldendal.Api.CoreData.Business.Util
{
    public class CoverImageUtil : ICoverImageUtil
    {
        private readonly IConfigurationManager _configurationManager;

        private string MultiMediaUrl => _configurationManager.MultiMediaUrl;

        private static string CoverFace => "CoverFace";

        private static string Original => "WH_Original";

        private readonly MMSImageServiceSoapClient _mmsImageServiceSoap;

        public CoverImageUtil(IConfigurationManager configurationManager, MMSImageServiceSoapClient mmsImageServiceSoap)
        {
            _configurationManager = configurationManager;
            _mmsImageServiceSoap = mmsImageServiceSoap;
        }

        public VariantImage GetProductImagesVariant(string isbn, DataScope dataScope = DataScope.Global, string organizationName = null)
        {
            try
            {
                var imageVariants = _mmsImageServiceSoap.GetVariantImagesAsync(isbn).Result?.Body?.GetVariantImagesResult?.ToList();

                if (imageVariants == null || imageVariants.Count == 0) throw new Exception();

                var productImages = imageVariants.Select(variant =>
                    {
                        var organizationNameToken = !string.IsNullOrEmpty(organizationName)
                            ? organizationName
                            : variant.OrganizationName;

                        int.TryParse(variant.Width, out var width);
                        int.TryParse(variant.Height, out var height);
                        var image =
                            new ProductImage
                            {
                                Url = $"{MultiMediaUrl}/{organizationNameToken}/{variant.Imagetoken}/{variant.Varianttoken}/{isbn}",
                                Width = width,
                                Height = height
                            };
                        return image;
                    }
                ).ToList();

                // original image is added at last
                var originalCoverImageUrl = GenerateOriginalCoverImageUrl(isbn, imageVariants, dataScope, organizationName);

                var variantImage = new VariantImage
                {
                    ProductImages = productImages,
                    OriginalCoverImageUrl = originalCoverImageUrl
                };
                return variantImage;
            }
            catch
            {
                var consumerToken = ConsumerTokenFromShop(dataScope);
                var originalCoverImageUrl = GenerateOriginalCoverImageUrl(isbn, consumerToken, organizationName);
                var variantImage = new VariantImage
                {
                    ProductImages = null,
                    OriginalCoverImageUrl = originalCoverImageUrl
                };
                return variantImage;
            }
        }

        public string GetOriginalImageUrl(string isbn)
        {
            try
            {
                var imageVariants = _mmsImageServiceSoap.GetVariantImagesAsync(isbn).Result?.Body?.GetVariantImagesResult?.ToList();
                if (imageVariants == null || imageVariants.Count == 0) return "";

                var originalCoverImageUrl = GenerateOriginalCoverImageUrl(isbn, imageVariants);

                return originalCoverImageUrl;
            }
            catch
            {
                var consumerToken = ConsumerTokenFromShop(DataScope.Global);
                return GenerateOriginalCoverImageUrl(isbn, consumerToken);
            }
        }

        private string ConsumerTokenFromShop(DataScope dataScope, string consumerToken = "Organisation")
        {
            if (consumerToken != "Organisation") return consumerToken;
            switch (dataScope)
            {
                case DataScope.Global:
                    return "Gyldendal";

                case DataScope.HansReitzelShop:
                    return "HansReitzelsForlag";

                case DataScope.MunksGaardShop:
                    return "MunksgaardDanmark";

                case DataScope.RosinantecoShop:
                    return "GB_Forlagene";

                case DataScope.GuShop:
                case DataScope.GyldendalDkShop:
                case DataScope.TradeGyldendalDk:
                    return "Gyldendal";

                case DataScope.GyldendalPlus:
                    return "BogklubRed";

                default:
                    throw new ArgumentOutOfRangeException(nameof(dataScope), dataScope, null);
            }
        }

        private string GenerateOriginalCoverImageUrl(string isbn, IEnumerable<VariantDto> imageVariants, DataScope dataScope = DataScope.Global, string organizationName = null)
        {
            if (!string.IsNullOrEmpty(organizationName))
            {
                return $"{MultiMediaUrl}/{organizationName}/{CoverFace}/{Original}/{isbn}";
            }

            var orgImage = imageVariants?.FirstOrDefault(x => (x.Imagetoken?.Equals(CoverFace, StringComparison.InvariantCultureIgnoreCase) ?? false) && x.Varianttoken == Original);

            string originalCoverImageUrl;
            if (orgImage == null || string.IsNullOrEmpty(orgImage.OrganizationName) || orgImage.OrganizationName == "Organisation")
            {
                originalCoverImageUrl = $"{MultiMediaUrl}/{ConsumerTokenFromShop(dataScope)}/{CoverFace}/{Original}/{isbn}";
            }
            else
            {
                originalCoverImageUrl = $"{MultiMediaUrl}/{orgImage.OrganizationName}/{CoverFace}/{Original}/{isbn}";
            }

            return originalCoverImageUrl;
        }

        private string GenerateOriginalCoverImageUrl(string isbn, string consumerToken, string organizationName = null)
        {
            return !string.IsNullOrEmpty(organizationName) ?
                $"{MultiMediaUrl}/{organizationName}/{CoverFace}/{Original}/{isbn}" :
                $"{MultiMediaUrl}/{consumerToken}/{CoverFace}/{Original}/{isbn}";
        }
    }
}