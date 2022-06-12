using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CoreData.Business.Porter;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.Logging;
using Gyldendal.Api.CoreData.Filters;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [IsGdprSafe(true)]
    public class WorkController : ApiController
    {
        private readonly ILogger _logger;

        private readonly IWorkFactory _workFactory;
        private readonly IWorkService _workService;
        private readonly IWorkDataProvider _workDataProvider;
        private readonly bool _isShadowMode;

        /// <param name="logger">The _loggers.</param>
        /// <param name="workFactory"></param>
        /// <param name="workDataProvider"></param>
        public WorkController(ILogger logger, IWorkFactory workFactory, IWorkDataProvider workDataProvider,
            IWorkService workService, IConfigurationManager configurationManager)
        {
            _logger = logger;
            _workFactory = workFactory;
            _workDataProvider = workDataProvider;
            _workService = workService;
            _isShadowMode = configurationManager.IsShadowMode;
        }

        /// <summary>
        /// Returns Ids of Deleted Works
        /// fromLastUpdated Time has to be send in query string see this link
        /// http://stackoverflow.com/questions/26659406/trouble-passing-datetime-parameter-to-web-service-in-get
        /// (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="fromLastUpdated"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Work/GetDeletedWork/{webShop}/{fromLastUpdated?}")]
        [ResponseType(typeof(List<string>))]
        public IHttpActionResult GetDeletedWorks(WebShop webShop, long? fromLastUpdated = null)
        {
            if (!_isShadowMode)
            {
                var fromLastUpdatedDate =
                    fromLastUpdated.HasValue ? new DateTime(fromLastUpdated.Value) : new DateTime();

                var result = _workFactory.GetDeletedWorks(webShop.ToDataScope(), fromLastUpdatedDate);
                return Ok(result);
            }
            else
            {
                throw new NotImplementedException("Endpoint is unused");
            }
        }

        /// <summary>
        /// Returns a Work object for the given Id and WebShop parameters (Source: Solr)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="workId"></param>
        /// <returns>Work Details</returns>
        [HttpGet]
        [Route("api/v1/Work/GetWorkById/{webShop}/{workId}")]
        [ResponseType(typeof(Work))]
        public IHttpActionResult GetWorkById(WebShop webShop, int workId)
        {
            return Ok(_workDataProvider.GetWorkById(new[] {webShop}, workId));
        }

        /// <summary>
        /// Returns a Work object for the given Id and WebShops parameters (Source: Solr)
        /// </summary>
        /// <param name="webShops"></param>
        /// <param name="workId"></param>
        /// <returns>Work Details</returns>
        [HttpPost]
        [Route("api/v1/Work/GetWorkById/{workId}")]
        [ResponseType(typeof(Work))]
        public IHttpActionResult GetWorkById(WebShop[] webShops, int workId)
        {
            return Ok(_workDataProvider.GetWorkById(webShops, workId));
        }

        /// <summary>
        /// Return Work Object and fill Product details (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="productType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Work/GetProductDetails/{webShop}/{productType}/{id}")]
        [ResponseType(typeof(GetProductDetailsResponse))]
        public async Task<IHttpActionResult> GetProductDetails(WebShop webShop, ProductType productType, string id)
        {
            if (!_isShadowMode)
            {
                var result = _workFactory.GetWorkByProductId(webShop.ToDataScope(), productType, id);
                return Ok(result);
            }
            else
            {
                var result = await _workService.GetWorkByProductIdAsync(webShop, productType, id);
                return Ok(result);
            }
        }

        /// <summary>
        /// Get list of work (Source: KD)
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="isbn"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/Work/GetScopeWorksByProductId/{dataScope}/{isbn}")]
        [ResponseType(typeof(GetScopeWorksByProductIdResponse))]
        public async Task<IHttpActionResult> GetScopeWorksByProductId(DataScope dataScope, string isbn)
        {
            if (!_isShadowMode)
            {
                var result = _workFactory.GetScopeWorksByProductId(dataScope, isbn);
                return Ok(result);
            }
            else
            {
                var result = await _workService.GetScopeWorksByProductIdAsync(dataScope, isbn);
                return Ok(result);
            }
        }
    }
}