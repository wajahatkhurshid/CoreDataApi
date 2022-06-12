using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
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
    /// Contributor V2 controller
    /// </summary>
    public class ContributorV2Controller : ApiController
    {
        private readonly IContributorFactory _contributorFactory;

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
        /// <param name="contributorDataProvider"></param>
        /// <param name="contributorService"></param>
        /// <param name="configurationManager"></param>
        public ContributorV2Controller(IContributorFactory contributorFactory, IContributorDataProvider contributorDataProvider, IContributorService contributorService, IConfigurationManager configurationManager)
        {
            _contributorFactory = contributorFactory;
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
        [ResponseType(typeof(ContributorDetailsV2))]
        [Route("api/v2/Contributor/GetContributor/{webShop}/{Id}")]
        public async Task<IHttpActionResult> GetContributorV2(WebShop webshop, string id)
        {
            if (!_isShadowMode)
            {
                var contributors = _contributorFactory.GetContributor<ContributorDetailsV2>(webshop.ToDataScope(), id);

                return Ok(contributors);
            }
            else
            {
                var contributors = await _contributorService.GetContributorDetailAsync(webshop, id);

                return Ok(contributors);
            }
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <param name="dataScope">Webshop</param>
        /// <returns>Contributor Details</returns>
        [HttpGet]
        [ResponseType(typeof(List<ContributorDetailsV2>))]
        [Route("api/v2/Contributor/GetContributorsByShopFromSolr/{dataScope}")]
        public IHttpActionResult GetContributorsByShopFromSolrV2(DataScope dataScope)
        {
            var contributors = _contributorDataProvider.GetContributorsByShopV2(dataScope);

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
        [ResponseType(typeof(List<ContributorDetailsV2>))]
        [Route("api/v2/Contributor/GetContributorsByIdsFromSolr/{dataScope}")]
        public IHttpActionResult GetContributorsByIdsFromSolrV2(DataScope dataScope, IList<string> ids)
        {
            var contributors = _contributorDataProvider.GetContributorsByIdsV2(ids, dataScope);

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
        [ResponseType(typeof(List<ContributorDetailsV2>))]
        [Route("api/v2/Contributor/GetContributorsByIds/{webShop}")]
        public IHttpActionResult GetContributorsByIdsV2(WebShop webShop, IList<string> ids)
        {
            var contributors = _contributorDataProvider.GetContributorsByIdsV2(ids, webShop);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetailsV2>))]
        [Route("api/v2/Contributor/Search")]
        public IHttpActionResult SearchV2(SearchContributorRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var contributors = _contributorDataProvider.Search<ContributorDetailsV2>(request);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given webshops (Source: Solr)
        /// </summary>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetailsV2>))]
        [Route("api/v2/Contributor/SearchByWebShops")]
        public IHttpActionResult SearchByWebShopsV2(SearchContributorByWebShopsRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var contributors = _contributorDataProvider.Search<ContributorDetailsV2>(request);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: Solr)
        /// </summary>
        /// <returns>Contributor Details</returns>
        [HttpPost]
        [NullValueFilter]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetailsV2>))]
        [Route("api/v2/Contributor/SearchBySearchName")]
        public IHttpActionResult SearchBySearchNameV2(SearchContributorRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var contributors = _contributorDataProvider.SearchBySearchName<ContributorDetailsV2>(request);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets all contributors for given type and webshop (Source: Solr)
        /// </summary>
        /// <param name="webshop">Webshop</param>
        /// <param name="searchRequest">Contributor search request</param>
        /// <returns>List of contributors</returns>
        [HttpPost]
        [ResponseType(typeof(SearchContributorResponse<ContributorDetailsV2>))]
        [Route("api/v2/Contributor/GetContributorsByType/{webShop}")]
        public IHttpActionResult GetContributorsByTypeV2(WebShop webshop, SearchContributorRequest searchRequest)
        {
            var contributors = _contributorFactory.GetContributors<ContributorDetailsV2>(webshop.ToDataScope(), searchRequest);

            return Ok(contributors);
        }

        /// <summary>
        /// Gets details of a contributor against given id (Source: KD)
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="id">Id</param>
        /// <returns>Contributor Details</returns>
        [HttpGet]
        [ResponseType(typeof(ContributorDetailsV2))]
        [Route("api/v2/Contributor/GetContributorDetail/{dataScope}/{Id}")]
        public async Task<IHttpActionResult> GetContributorDetailV2(DataScope dataScope, string id)
        {
            if (!_isShadowMode)
            {
                var contributors = _contributorFactory.GetContributor<ContributorDetailsV2>(dataScope, id);

                return Ok(contributors);
            }
            else
            {
                var contributors = await _contributorService.GetContributorDetailAsync(dataScope.ToFirstWebShop(), id);

                return Ok(contributors);
            }
        }
    }
}