using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Business.Util;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;
using Gyldendal.Api.CoreData.Filters;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Contributor Controller
    /// </summary>
    public class ContributorController : ApiController
    {
        private readonly IContributorFactory _contributorFactory;

        private readonly IKoncernDataUtils _koncernDataUtils;

        private readonly IContributorDataProvider _contributorDataProvider;

        private readonly IContributorService _contributorService;

        /// <summary>
        /// Used to switch CoreData API between running against Porter(GPM) if true and standard KD if false
        /// </summary>
        private readonly bool _isShadowMode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contributorFactory">Contributor Factory</param>
        /// <param name="koncernDataUtils"></param>
        /// <param name="contributorDataProvider"></param>
        /// <param name="contributorService"></param>
        /// <param name="configurationManager"></param>
        public ContributorController(IContributorFactory contributorFactory,
            IKoncernDataUtils koncernDataUtils,
            IContributorDataProvider contributorDataProvider,
            IContributorService contributorService,
            IConfigurationManager configurationManager)
        {
            _contributorFactory = contributorFactory;
            _koncernDataUtils = koncernDataUtils;
            _contributorDataProvider = contributorDataProvider;
            _contributorService = contributorService;
            _isShadowMode = configurationManager.IsShadowMode;
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: KD)
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        [HttpGet]
        [ResponseType(typeof(ContributorDetails))]
        [Route("api/v1/Contributor/GetContributor/{webShop}/{Id}")]
        public async Task<IHttpActionResult> GetContributor(WebShop webshop, string id)
        {

            if (!_isShadowMode)
            {
                var contributors = _contributorFactory.GetContributor<ContributorDetails>(webshop.ToDataScope(), id);

                return Ok(contributors);
            }
            else
            {
                var contributors = await _contributorService.GetContributorByIdAsync(webshop, id);
                return Ok(contributors);
            }

        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <param name="dataScope">Webshop</param>
        /// <returns>Contributor Details</returns>
        [HttpGet]
        [ResponseType(typeof(List<ContributorDetails>))]
        [Route("api/v1/Contributor/GetContributorsByShopFromSolr/{dataScope}")]
        public IHttpActionResult GetContributorsByShopFromSolr(DataScope dataScope)
        {
            var contributors = _contributorDataProvider.GetContributorsByShop(dataScope);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="dataScope">dataScope</param>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(List<ContributorDetails>))]
        [Route("api/v1/Contributor/GetContributorsByIdsFromSolr/{dataScope}")]
        public IHttpActionResult GetContributorsByIdsFromSolr(DataScope dataScope, IList<string> ids)
        {
            var contributors = _contributorDataProvider.GetContributorsByIds(ids, dataScope);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="webShop">dataScope</param>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(List<ContributorDetails>))]
        [Route("api/v1/Contributor/GetContributorsByIds/{webShop}")]
        public IHttpActionResult GetContributorsByIds(WebShop webShop, IList<string> ids)
        {
            var contributors = _contributorDataProvider.GetContributorsByIds(ids, webShop);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetails>))]
        [Route("api/v1/Contributor/Search")]
        public IHttpActionResult Search(SearchContributorRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var contributors = _contributorDataProvider.Search<ContributorDetails>(request);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given webshops (Source: Solr)
        /// </summary>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetails>))]
        [Route("api/v1/Contributor/SearchByWebShops")]
        public IHttpActionResult SearchByWebShops(SearchContributorByWebShopsRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var contributors = _contributorDataProvider.Search<ContributorDetails>(request);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetails>))]
        [Route("api/v1/Contributor/SearchBySearchName")]
        public IHttpActionResult SearchBySearchName(SearchContributorRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var contributors = _contributorDataProvider.SearchBySearchName<ContributorDetails>(request);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets all contributors for given type and webshop (Source: Solr)
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>List of contributors</returns>
        [HttpPost]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetails>))]
        [Route("api/v1/Contributor/GetContributorsByType/{webShop}")]
        public IHttpActionResult GetContributorsByType(WebShop webshop, SearchContributorRequest searchRequest)
        {
            var contributors = _contributorFactory.GetContributors<ContributorDetails>(webshop.ToDataScope(), searchRequest);

            return Ok(contributors);
        }

        /// <summary>
        /// Returns the number of contributors updated after the given Ticks value (Source: KD).
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Contributor/GetUpdatedContributorsCount/{dataScope}/{updatedAfterTicks}")]
        public async Task<IHttpActionResult> GetUpdatedContributorsCount(DataScope dataScope, long updatedAfterTicks)
        {
            var updateAfterDateTime = new DateTime(updatedAfterTicks);
            if (!_isShadowMode)
            {
                var result = _contributorFactory.GetUpdatedContributorsCount(dataScope, updateAfterDateTime);
                return Ok(result);
            }
            else
            {
                var result = await _contributorService.GetUpdatedContributorsCountAsync(updatedAfterTicks, dataScope.ToFirstWebShop());
                return Ok(result);
            }
        }

        /// <summary>
        /// Returns the asked page of ContributorupdatedInfo objects, for each contributor,
        /// that was updated after the given DateTime, in KoncernDataWebShops database in KD.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Contributor/GetContributorsUpdateInfo/{dataScope}/{updatedAfterTicks}/{pageIndex}/{pageSize}")]
        public async Task<IHttpActionResult> GetContributorsUpdateInfo(DataScope dataScope, long updatedAfterTicks, int pageIndex, int pageSize)
        {
            if (!_isShadowMode)
            {
                var updatedAfterDateTime = new DateTime(updatedAfterTicks);
                return Ok(_contributorFactory.GetContributorsUpdateInfo(dataScope, updatedAfterDateTime, pageIndex,
                    pageSize));
            }

            var result = await _contributorService.GetContributorsUpdateInfo(updatedAfterTicks, pageIndex, pageSize, dataScope.ToFirstWebShop());
            return Ok(result);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: KD)
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        [HttpGet]
        [ResponseType(typeof(ContributorDetails))]
        [Route("api/v1/Contributor/GetContributorDetail/{dataScope}/{Id}")]
        public async Task<IHttpActionResult> GetContributorDetail(DataScope dataScope, string id)
        {
            if (!_isShadowMode)
            {
                var contributors = _contributorFactory.GetContributor<ContributorDetails>(dataScope, id);

                return Ok(contributors);
            }
            else
            {
                var contributors = await _contributorService.GetContributorByIdAsync(dataScope.ToFirstWebShop(), id);
                return Ok(contributors);
            }

        }

        /// <summary>
        /// Determines if the contributor data will be available in Koncerndata for the next x minutes  (Source: KD).
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="xMinutes">The number of minutes by which the contributor data will be available or not.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Contributor/IsContributorDataAvailableForXMinutes/{dataScope}/{xMinutes}")]
        public async Task<IHttpActionResult> IsContributorDataAvailableForXMinutes(DataScope dataScope, short xMinutes)
        {
            if (!_isShadowMode)
            {
                return Ok(_koncernDataUtils.IsContributorDataAvailableForXMinutes(dataScope, xMinutes));
            }

            return Ok(await _contributorService.IsContributorDataAvailable());
        }

        /// <summary>
        /// Returns the latest update info for synchronization purposes, for the given contributor id and data scope. (Source: KD)
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="contributorId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Contributor/GetContributorUpdateInfo/{dataScope}/{contributorId}")]
        [ResponseType(typeof(ContributorUpdateInfo))]
        public async Task<IHttpActionResult> GetContributorUpdateInfo(DataScope dataScope, string contributorId)
        {
            if (!_isShadowMode)
            {
                var result = _contributorFactory.GetContributorUpdateInfo(dataScope, contributorId);
                return Ok(result);
            }

            var response = await _contributorService.GetContributorUpdateInfo(contributorId, dataScope.ToFirstWebShop());
            return Ok(response);

        }
    }
}