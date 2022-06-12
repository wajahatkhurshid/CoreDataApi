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
using Gyldendal.Api.CoreData.Filters;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    ///
    /// </summary>
    [IsGdprSafe(true)]
    public class WorkReviewsController : ApiController
    {
        private readonly IKoncernDataUtils _koncernDataUtils;

        private readonly IWorkReviewsFactory _workReviewsFactory;

        private readonly IWorkReviewDataProvider _workReviewDataProvider;

        private readonly IWorkReviewService _workReviewService;

        /// <summary>
        /// Used to switch CoreData API between running against Porter(GPM) if true and standard KD if false
        /// </summary>
        private readonly bool _isShadowMode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workReviewsFactory"></param>
        /// <param name="koncernDataUtils"></param>
        /// <param name="workReviewDataProvider"></param>
        /// <param name="workReviewService"></param>
        /// <param name="configurationManager"></param>
        public WorkReviewsController(IWorkReviewsFactory workReviewsFactory, IKoncernDataUtils koncernDataUtils, IWorkReviewDataProvider workReviewDataProvider, IWorkReviewService workReviewService, IConfigurationManager configurationManager)
        {
            _workReviewsFactory = workReviewsFactory;
            _koncernDataUtils = koncernDataUtils;
            _workReviewDataProvider = workReviewDataProvider;
            _workReviewService = workReviewService;
            _isShadowMode = configurationManager.IsShadowMode;
        }

        /// <summary>
        /// Returns the number of WorkReviews updated after the given Ticks value (Source: KD)
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/WorkReviews/GetUpdatedWorkReviewsCount/{dataScope}/{updatedAfterTicks}")]
        public async Task<IHttpActionResult> GetUpdatedWorkReviewsCount(DataScope dataScope, long updatedAfterTicks)
        {
            if (!_isShadowMode)
            {
                var updateAfterDateTime = new DateTime(updatedAfterTicks);
                var result = _workReviewsFactory.GetUpdatedWorkReviewsCount(dataScope, updateAfterDateTime);

                return Ok(result);
            }
            else
            {
                var result = await _workReviewService.GetUpdatedWorkReviewsCountAsync(updatedAfterTicks);
                return Ok(result);
            }
        }

        /// <summary>
        /// Gets details of a WorkReview against given id (Source: Solr)
        /// </summary>
        /// <param name="dataScope">WebShop</param>
        /// <returns>WorkReview Details</returns>
        [HttpGet]
        [ResponseType(typeof(List<WorkReview>))]
        [Route("api/v1/WorkReviews/GetWorkReviewsByShopFromSolr/{dataScope}")]
        public IHttpActionResult GetWorkReviewsByShopFromSolr(DataScope dataScope)
        {
            var workReviews = _workReviewDataProvider.GetWorkReviewsByShop(dataScope);

            return Ok(workReviews);
        }

        /// <summary>
        /// Gets work reviews for given work id (Source: Solr)
        /// </summary>
        /// <param name="workId"></param>
        /// <param name="webShop"></param>
        /// <returns>WorkReview</returns>
        [HttpGet]
        [ResponseType(typeof(List<WorkReview>))]
        [Route("api/v1/WorkReviews/GetWorkReviewsByWorkIdFromSolr")]
        // TODO: remove old Route and optional param, once everyone is switched to new Route.
        [Route("api/v1/WorkReviews/GetWorkReviewsByWorkIdFromSolr/{workId}")]
        public IHttpActionResult GetWorkReviewsByWorkIdFromSolr(int workId, WebShop webShop = WebShop.None)
        {
            var workReview = _workReviewDataProvider.GetWorkReviewsByWorkId(workId, webShop);

            return Ok(workReview);
        }

        /// <summary>
        /// Returns the asked page of WorkReviewUpdatedInfo objects, for each WorkReview,
        /// that was updated after the given DateTime, in KD.
        /// </summary>
        /// <param name="dataScope"></param>
        /// <param name="updatedAfterTicks"></param>
        /// <param name="pageIndex">Minimum value 1.</param>
        /// <param name="pageSize">Minimum value 1.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/WorkReviews/GetWorkReviewsUpdateInfo/{dataScope}/{updatedAfterTicks}/{pageIndex}/{pageSize}")]
        public async Task<IHttpActionResult> GetWorkReviewsUpdateInfo(DataScope dataScope, long updatedAfterTicks, int pageIndex, int pageSize)
        {
            if (!_isShadowMode)
            {
                var updatedAfterDateTime = new DateTime(updatedAfterTicks);
                return Ok(_workReviewsFactory.GetWorkReviewsUpdateInfo(dataScope, updatedAfterDateTime, pageIndex,
                    pageSize));
            }
            else
            {
                var result = await _workReviewService.GetWorkReviewsUpdateInfoAsync(updatedAfterTicks, pageIndex, pageSize);

                return Ok(result);
            }
        }

        /// <summary>
        /// Gets details of a WorkReview against given id (Source: KD)
        /// </summary>
        /// <param name="dataScope">dataScope</param>
        /// <param name="id">Id</param>
        /// <returns>WorkReview Details</returns>
        [HttpGet]
        [ResponseType(typeof(WorkReview))]
        [Route("api/v1/WorkReviews/GetWorkReview/{dataScope}/{Id}")]
        public async Task<IHttpActionResult> GetWorkReview(DataScope dataScope, int id)
        {
            if (!_isShadowMode)
            {
                var workReviews = _workReviewsFactory.GetWorkReview(dataScope, id);

                return Ok(workReviews);
            }
            else
            {
                var workReviews = await _workReviewService.GetWorkReviewByIdAsync(dataScope.ToFirstWebShop(), id.ToString());

                return Ok(workReviews);
            }

        }

        /// <summary>
        /// Determines if the WorkReviews data will be available for the next x minutes in KD.
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="xMinutes">The number of minutes by which the WorkReviews data will be available or not.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/WorkReviews/IsWorkReviewsDataAvailableForXMinutes/{webShop}/{xMinutes}")]
        public async Task<IHttpActionResult> IsWorkReviewsDataAvailableForXMinutes(WebShop webShop, short xMinutes)
        {
            if (!_isShadowMode)
            {
                return Ok(_koncernDataUtils.IsShopDataAvailableForXMinutes(webShop, xMinutes));
            }

            return Ok(await _workReviewService.IsWorkReviewDataAvailable());
        }

        /// <summary>
        /// Get list of WebShops against isbn
        /// </summary>
        /// <param name="isbn"></param>
        /// <param name="dataScope"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/WorkReviews/GetWorkReviews/{dataScope}/{isbn}")]
        public async Task<IHttpActionResult> GetWorkReviews(string isbn, DataScope dataScope)
        {
            if (_isShadowMode)
            {
                var workReviews = await _workReviewService.GetWorkReviewsByIsbn(dataScope.ToFirstWebShop(), isbn);

                return Ok(workReviews);
            }

            return Ok(_workReviewsFactory.GetWorkReviews(isbn, dataScope));
        }
    }
}