using System.Collections.Generic;
using System.Linq;

// ReSharper disable StringLiteralTypo

namespace Gyldendal.Api.CoreData.Common.ConfigurationManager
{
    /// <summary>
    /// Class for Reading Configuration from webconfig
    /// </summary>
    public class ConfigurationManager : BaseConfigurationManager, IConfigurationManager
    {
        public bool IsShadowMode
        {
            get
            {
                var shadowModeConfigValue = GetConfigValue("IsShadowMode");
                var isParseable = bool.TryParse(shadowModeConfigValue, out var isShadowMode);
                return isParseable && isShadowMode;
            }
        }

        public string MediaProviderUrl => GetConfigValue("MediaProviderUrl");

        public List<string> DownloadableMaterialMediaTypes =>
            GetConfigValuesListFromCsvString("DownloadableMaterialMediaTypes").ToList();

        public string ShopServicesUrl => GetConfigValue("shopservices");

        /// <summary>
        /// MultiMedia Url Require to generate Cover Images of the Product
        /// </summary>
        public string MultiMediaUrl => GetConfigValue("MultimediaServer");

        /// <summary>
        /// Gets the base image Url for a System and Series, the consumer just has to append the system/series id and the image extension.
        /// </summary>
        public string SystemSerieImageBaseUrl => GetConfigValue("SystemSerieImageBaseUrl");

        /// <summary>
        /// Gets the image extension for System/Series images.
        /// </summary>
        public string SystemSerieImageExt => GetConfigValue("SystemSerieImageExt");

        /// <summary>
        /// cover Image varients of the Gu webshop
        /// </summary>
        public List<string> GuImageVarients => GetConfigValuesListFromCsvString("guimagevairent").ToList();

        /// <summary>
        /// Cover Image Varients for the Hr webshop
        /// </summary>
        public List<string> HrImageVarients => GetConfigValuesListFromCsvString("hrimagevairent").ToList();

        /// <summary>
        /// cover Image Varient for the Munk webshop
        /// </summary>
        public List<string> MunkImageVarients => GetConfigValuesListFromCsvString("munkimagevairent").ToList();

        /// <summary>
        /// GU Consumer
        /// </summary>
        public string GuConsumer => GetConfigValue("gumultimediaconsumer");

        /// <summary>
        /// Hr Consumer
        /// </summary>
        public string HrConsumer => GetConfigValue("hrmultimediaconsumer");

        /// <summary>
        /// Munk Consumer
        /// </summary>
        public string MunkConsumer => GetConfigValue("munkmultimediaconsumer");

        /// <summary>
        /// List of Physical Media Types
        /// </summary>
        public string[] PhysicalMediaTypes 
        {
            get
            {
                var shadowModeConfigValue = GetConfigValue("IsShadowMode");
                bool.TryParse(shadowModeConfigValue, out var isShadowMode);
                return isShadowMode
                    ? GetConfigValuesListFromCsvString("shadowPhysicalProductsMediatype").ToArray()
                    : GetConfigValuesListFromCsvString("physicalproductsmediatype").ToArray();
            }
        }

        /// <summary>
        /// The number of hours after which the KoncernDataWebShops database gets refreshed.
        /// </summary>
        public int KdRefreshFrequencyInMinutes => GetConfigValue("KDRefreshFrequencyInMinutes", 0);

        /// <summary>
        /// Get pulsen services Url.
        /// </summary>
        public string PulsenServicesUrl => GetConfigValue("PulsenServicesApiUrl");

        public string CoreDataAgentImportStateFilesPath => GetConfigValue("CoreDataAgentImportStateFilesPath");

        public string MmmsWcfServiceUrl => GetConfigValue("MMSWCFSERVICEURL");

        public string Contentful_SpaceId => GetConfigValue("Contentful_SpaceId");

        public string Contentful_Environment => GetConfigValue("Contentful_Environment");

        public string Contentful_PreviewApiKey => GetConfigValue("Contentful_PreviewApiKey");

        public string Contentful_DeliveryApiKey => GetConfigValue("Contentful_DeliveryApiKey");

        public bool UseRequestWebshop
        {
            get
            {
                var useRequestWebshopValues = GetConfigValue("AbsoluteSearchV1_UseRequestWebshop");
                var isParseable = bool.TryParse(useRequestWebshopValues, out var isUseRequestWebshop);
                return isParseable && isUseRequestWebshop;
            }
        }
    }
}