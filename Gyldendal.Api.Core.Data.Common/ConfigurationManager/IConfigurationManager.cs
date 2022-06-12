using System.Collections.Generic;

namespace Gyldendal.Api.CoreData.Common.ConfigurationManager
{
    /// <summary>
    /// interface for the Configuration Manager
    /// </summary>
    public interface IConfigurationManager
    {
        bool IsShadowMode { get; }
        /// <summary>
        /// Media Provider base Url used to created secured resource link
        /// </summary>
        string MediaProviderUrl { get; }

        List<string> DownloadableMaterialMediaTypes { get; }

        /// <summary>
        /// ShopService Url
        /// </summary>
        string ShopServicesUrl { get; }

        /// <summary>
        /// MultiMedia Url Require to generate Cover Images of the Product
        /// </summary>
        string MultiMediaUrl { get; }

        /// <summary>
        /// Gets the base image Url for a System and Series, the consumer just has to append the system/series id and the image extension.
        /// </summary>
        string SystemSerieImageBaseUrl { get; }

        /// <summary>
        /// Gets the image extension for System/Series images.
        /// </summary>
        string SystemSerieImageExt { get; }

        /// <summary>
        /// cover Image varients of the Gu webshop
        /// </summary>
        List<string> GuImageVarients { get; }

        /// <summary>
        /// Cover Image Varients for the Hr webshop
        /// </summary>
        List<string> HrImageVarients { get; }

        /// <summary>
        /// cover Image Varient for the Munk webshop
        /// </summary>
        List<string> MunkImageVarients { get; }

        /// <summary>
        /// GU Consumer
        /// </summary>
        string GuConsumer { get; }

        /// <summary>
        /// Hr Consumer
        /// </summary>
        string HrConsumer { get; }

        /// <summary>
        /// Munk Consumer
        /// </summary>
        string MunkConsumer { get; }

        /// <summary>
        /// List of physical Media Types
        /// </summary>
        string[] PhysicalMediaTypes { get; }

        /// <summary>
        /// The number of hours after which the KoncernDataWebShops database gets refreshed.
        /// </summary>
        int KdRefreshFrequencyInMinutes { get; }

        /// <summary>
        /// Get pulsen services Url.
        /// </summary>
        string PulsenServicesUrl { get; }

        string CoreDataAgentImportStateFilesPath { get; }

        string MmmsWcfServiceUrl { get; }

        string Contentful_SpaceId { get; }

        string Contentful_Environment { get; }

        string Contentful_PreviewApiKey { get; }

        string Contentful_DeliveryApiKey { get; }

        bool UseRequestWebshop { get; }
    }
}