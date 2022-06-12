using System.Threading.Tasks;
using System.Web.Http;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Filters;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Returns System and Series Information to the client
    /// </summary>
    [IsGdprSafe(true)]
    public class SerieSystemController : ApiController
    {
        private readonly ISystemSeriesFactory _systemSeriesFactory;
        private readonly ISeriesService _seriesService;
        /// <summary>
        /// Used to switch CoreData API between running against Porter(GPM) if true and standard KD if false
        /// </summary>
        private readonly bool _isShadowMode;

        /// <summary>
        /// Constructor of the SerieSystem Controller
        /// </summary>
        /// <param name="systemSeriesFactory"></param>
        /// <param name="seriesService"></param>
        public SerieSystemController(ISystemSeriesFactory systemSeriesFactory, ISeriesService seriesService)
        {
            _systemSeriesFactory = systemSeriesFactory;
            _seriesService = seriesService;
        }

        /// <summary>
        ///  Get a collection of Systems for the specified WebSite (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="serieRequest"></param>
        /// <returns>List of Systems</returns>
        [HttpPost]
        [Route("api/v1/SystemSeries/Series/{webShop}")]
        public async Task<IHttpActionResult> GetSeries(WebShop webShop, GetSeriesRequest serieRequest)
        {
            if (!_isShadowMode)
            {
                var result = _systemSeriesFactory.GetSeries(webShop.ToDataScope(), serieRequest);
                return Ok(result);
            }
            else
            {
                var result = await _seriesService.GetSeriesAsync(webShop, serieRequest);
                return Ok(result);
            }
        }

        /// <summary>
        ///  Get a collection of Systems for the specified WebSite (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="serieId"></param>
        /// <returns>List of Systems</returns>
        [HttpGet]
        [Route("api/v1/SystemSeries/SerieById/{webShop}/{serieId}")]
        public async Task<IHttpActionResult> GetSerieById(WebShop webShop, int serieId)
        {
            if (!_isShadowMode)
            {
                var result = _systemSeriesFactory.GetSerieById(webShop.ToDataScope(), serieId);
                return Ok(result);
            }
            else
            {
                var result = await _seriesService.GetSeriesByIdAsync(webShop, serieId);
                return Ok(result);
            }
        }
    }
}