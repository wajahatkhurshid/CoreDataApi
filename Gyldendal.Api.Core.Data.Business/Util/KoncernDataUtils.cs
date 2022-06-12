using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;

namespace Gyldendal.Api.CoreData.Business.Util
{
    /// <summary>
    /// Provides operations to do miscellaneous Koncerndata operations related to all Web Shops.
    /// </summary>
    public class KoncernDataUtils : IKoncernDataUtils
    {
        private readonly koncerndata_webshops_Entities _kdEntities;

        private readonly IConfigurationManager _configManager;

        private readonly ILogger _logger;

        private readonly ICoverImageUtil _coverImage;

        public KoncernDataUtils(koncerndata_webshops_Entities kdEntities, IConfigurationManager configManager,
            ILogger logger, ICoverImageUtil coverImage)
        {
            _kdEntities = kdEntities;
            _configManager = configManager;
            _logger = logger;
            _coverImage = coverImage;
        }

        /// <summary>
        /// Get List of Products From Koncerndata
        /// </summary>
        /// <param name="webshop"></param>
        /// <param name="isbnList">ISBN list</param>
        /// <param name="getImageUrl"></param>
        /// <returns></returns>
        public IEnumerable<ProductBasicData> GetLicensedProductsByIsbn(WebShop webshop, IList<string> isbnList,
            bool getImageUrl = true)
        {
            _logger.Info($"GetProductsByIsbns: isbnList: {isbnList}.", isGdprSafe: true);

            var vareList = _kdEntities.varer.Where(x => isbnList.Contains(x.id) || isbnList.Contains(x.varenummer));

            var supplerendeMaterialers =
                _kdEntities.supplerende_materialer.Where(s => s.kd_slettet == 0 && s.is_secured == true);

            var combinedResult = vareList
                .GroupJoin(supplerendeMaterialers, v => v.id, s => s.vare,
                    (v, s) => new {Vare = v, SupplementeryMaterial = s})
                .SelectMany(x => x.SupplementeryMaterial.DefaultIfEmpty(),
                    (x, y) => new {x.Vare, HasSecuredMaterial = (y != null)});

            // Select only required columns
            return combinedResult.Select(x => new
            {
                x.HasSecuredMaterial,
                x.Vare.id,
                x.Vare.medietype,
                x.Vare.titel,
                x.Vare.undertitel,
                x.Vare.varenummer,
                x.Vare.webadresse
            }).ToList().Select(x => new ProductBasicData
            {
                Isbn = x.id,
                Title = x.titel,
                Subtitle = x.undertitel,
                Isbn10 = x.varenummer,
                ImageUrl = getImageUrl ? _coverImage.GetOriginalImageUrl(x.id) : null,
                MediaType = x.medietype,
                DigitalProductLink =
                    Repositories.Common.ModelsMapping.GetDigitalProductUrl(webshop, x.id, x.medietype, x.webadresse,
                        _configManager)
            });
        }

        /// <summary>
        /// Get Product access type by isbn From Koncerndata
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns></returns>
        public ProductAccessControlType GetProductAccessTypeByIsbn(string isbn)
        {
            _logger.Info($"GetProductAccessTypeByIsbn: isbn: {isbn}.", isGdprSafe: true);
            var vare = _kdEntities.varer.FirstOrDefault(x =>
                x.id.Equals(isbn, StringComparison.CurrentCultureIgnoreCase) ||
                x.varenummer.Equals(isbn, StringComparison.CurrentCultureIgnoreCase));
            switch (vare?.adgangskontrol)
            {
                case "EKEY":
                    return ProductAccessControlType.Ekey;

                default:
                    return ProductAccessControlType.Unic;
            }
        }

        /// <summary>
        /// Determines if the contributor's data will be available in koncerndata for the next x miutes.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="xMinutes">The number of minutes by which the contributor will be avilable or not.</param>
        /// <returns></returns>
        public bool IsContributorDataAvailableForXMinutes(DataScope dataScope, short xMinutes)
        {
            var kdSemaphores = _kdEntities.DEA_KDWS_Semaphores.ToList();
            // Get Contributor's data availability status.
            if (!kdSemaphores.Any())
            {
                _logger.Info("IsContributorDataAvailableForXMinutes: Semaphore not implemented.", isGdprSafe: true);
                return false;
            }

            if (kdSemaphores.Any(x => x.ExecStatus))
            {
                // KD job is executing right now.
                _logger.Info("IsContributorDataAvailableForXMinutes: KD job is executing right now.", isGdprSafe: true);
                return false;
            }

            if (kdSemaphores.Any(x => x.ErrorCode == 1))
            {
                // The last execution of KD job was unsuccessful.
                _logger.Info("IsContributorDataAvailableForXMinutes: The last execution of KD job was unsuccessful.", isGdprSafe: true);
                return false;
            }

            // Calculate next execution time.
            var minlastJobStartTime = kdSemaphores.Min(x => x.StartTimeStamp);
            if (minlastJobStartTime == null)
            {
                _logger.Info("Job start time is not available", isGdprSafe: true);
                return false;
            }

            _logger.Info(
                $"IsContributorDataAvailableForXMinutes: minimmum of lastJobStartTime: {minlastJobStartTime}.", isGdprSafe: true);
            var nextKdJobToStartAt = minlastJobStartTime.Value.AddMinutes(_configManager.KdRefreshFrequencyInMinutes);
            _logger.Info(
                $"IsContributorDataAvailableForXMinutes: KdRefreshFrequencyInMinutes: {_configManager.KdRefreshFrequencyInMinutes}.", isGdprSafe: true);
            _logger.Info($"IsContributorDataAvailableForXMinutes: nextKdJobToStartAt: {nextKdJobToStartAt}.", isGdprSafe: true);

            // Calculate the time by which data should be available.
            var dataNeededTill = DateTime.Now.AddMinutes(xMinutes);
            _logger.Info($"IsContributorDataAvailableForXMinutes: dataNeededTill: {dataNeededTill}.", isGdprSafe: true);

            // Return True if dataNeededTill is less than nextKdJobToStartAt;
            _logger.Info(
                $"IsContributorDataAvailableForXMinutes: dataNeededTill < nextKdJobToStartAt: {dataNeededTill < nextKdJobToStartAt}.", isGdprSafe: true);
            return dataNeededTill < nextKdJobToStartAt;
        }

        /// <summary>
        /// returns ifilserver url of the given isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <returns>
        /// Location (URL) of first secured material of given isbn or null if not found
        /// </returns>
        /// <remarks>
        /// Bug Id 10888 proposed solution point 4
        /// Fix Media Provider / CoreData to return secured attachment of the product instead of first attachment found.
        /// </remarks>
        public string GetSupplementaryData(string isbn)
        {
            var supplementaryData = _kdEntities.supplerende_materialer.FirstOrDefault(s =>
                s.vare.Equals(isbn, StringComparison.CurrentCultureIgnoreCase) &&
                s.kd_slettet == 0 &&
                s.is_secured == true);

            return supplementaryData?.dmz_url;
        }

        /// <summary>
        /// Determines if the Web Shop's data will be available in Koncerndata for the next x minutes.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="xMinutes">The number of minutes by which the Web Shop data will be available or not.</param>
        /// <returns></returns>
        public bool IsShopDataAvailableForXMinutes(WebShop webShop, short xMinutes)
        {
            // Replacing new GDK-26 with old GDK-8 as KD is not maintaining semaphore for new GDK-26 that's why when CoreDataAgent will request for GDK-26 semaphore, the old GDK-8 response will be returned.
            // This needs to be catered in some technical debt PBI to implement it properly.
            if (webShop == WebShop.TradeGyldendalDk)
            {
                _logger.Info("Replacing new GDK-26 with old GDK-8 as KD is not maintaining semaphore for new GDK-26 that's why returning the old GDK-8 response.", isGdprSafe: true);
                webShop = WebShop.GyldendalDk;
            }

            var webShopId = (int) webShop;

            // Get Web Shop data availability status.
            if (!(_kdEntities.DEA_KDWS_Semaphores.Any(x => x.WebShopId == webShopId)))
            {
                _logger.Info(
                    $"IsShopDataAvailableForXMinutes: Web Shop: {webShop}, Semaphore not implemented for the requested web shop.", isGdprSafe: true);
                return true;
            }

            // Get Web Shop data availability status.
            var webShopSemaphore = _kdEntities.DEA_KDWS_Semaphores.FirstOrDefault(x =>
                    x.WebShopId == webShopId && // Semaphore is of the asked Web Shop.
                    !(x.ExecStatus) && // KD job is not executing right now.
                    x.ErrorCode == 0 // The last execution of KD job was successful.
            );

            // We should have a Semaphore record for the above query and StartTimeStamp should also exist.
            if (webShopSemaphore?.StartTimeStamp == null)
            {
                _logger.Info($"IsShopDataAvailableForXMinutes: Web Shop: {webShop}, StartTimeStamp is null.", isGdprSafe: true);
                return false;
            }

            var lastJobStartTime = webShopSemaphore.StartTimeStamp.Value;
            _logger.Info($"IsShopDataAvailableForXMinutes: Web Shop: {webShop}, lastJobStartTime: {lastJobStartTime}.", isGdprSafe: true);

            // Calculate next execution time.
            var nextKdJobToStartAt = lastJobStartTime.AddMinutes(_configManager.KdRefreshFrequencyInMinutes);
            _logger.Info(
                $"IsShopDataAvailableForXMinutes: Web Shop: {webShop}, KdRefreshFrequencyInMinutes: {_configManager.KdRefreshFrequencyInMinutes}.", isGdprSafe: true);
            _logger.Info(
                $"IsShopDataAvailableForXMinutes: Web Shop: {webShop}, nextKdJobToStartAt: {nextKdJobToStartAt}.", isGdprSafe: true);

            // Calculate the time by which data should be available.
            var dataNeededTill = DateTime.Now.AddMinutes(xMinutes);
            _logger.Info($"IsShopDataAvailableForXMinutes: Web Shop: {webShop}, dataNeededTill: {dataNeededTill}.", isGdprSafe: true);

            // Return True if dataNeededTill is less than nextKdJobToStartAt;
            _logger.Info(
                $"IsShopDataAvailableForXMinutes: Web Shop: {webShop}, dataNeededTill < nextKdJobToStartAt: {dataNeededTill < nextKdJobToStartAt}.", isGdprSafe: true);
            return dataNeededTill < nextKdJobToStartAt;
        }

        public async Task RegisterContributorChangeFromThirdPartyAsync(DataScope dataScope, string source, string contributorId)
        {
            ValidateInputParameters(source, contributorId);

            var entry = _kdEntities.DEA_KDWS_3rdPartyContributorsLog.FirstOrDefault(a => a.forfatterID == contributorId
                                                                                         && a.Source == source
                                                                                         && a.DataScope == (int) dataScope);

            if (entry == null)
            {
                entry = new DEA_KDWS_3rdPartyContributorsLog();

                _kdEntities.DEA_KDWS_3rdPartyContributorsLog.Add(entry);
            }

            entry.Source = source;
            entry.forfatterID = contributorId;
            entry.Action = "Update";
            entry.CreatedDate = DateTime.Now;
            entry.DataScope = (int) dataScope;

            await _kdEntities.SaveChangesAsync();
        }

        private void ValidateInputParameters(string source, string contributorId)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(contributorId))
                throw new ArgumentNullException(
                    $"Input parameters Source: {source}, ContributorId: {contributorId} can not be null");
        }
    }
}