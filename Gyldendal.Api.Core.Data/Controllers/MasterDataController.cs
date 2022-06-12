using System.Threading.Tasks;
using System.Web.Http;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories;
using Gyldendal.Api.CoreData.Filters;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Returns Master Data to the Client
    /// </summary>
    [IsGdprSafe(true)]
    public class MasterDataController : ApiController
    {
        private readonly IMasterDataFactory _masterDataFactory;
        private readonly IMasterDataService _masterDataService;
        /// <summary>
        /// Used to switch CoreData API between running against Porter(GPM) if true and standard KD if false
        /// </summary>
        private readonly bool _isShadowMode;

        /// <summary>
        /// Constructor of the MediaType Controller
        /// </summary>
        public MasterDataController(IMasterDataFactory masterDataFactory, IMasterDataService masterDataService,
            IConfigurationManager configurationManager)
        {
            _masterDataFactory = masterDataFactory;
            _masterDataService = masterDataService;
            _isShadowMode = configurationManager.IsShadowMode;
        }

        /// <summary>
        /// Get a collection of media-types for the specified WebSite (Source: KD)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/MasterData/MediaTypes/{webShop}")]
        public async Task<IHttpActionResult> GetMediaTypes(WebShop webShop)
        {
            if (!_isShadowMode)
            {
                var result = _masterDataFactory.GetMediaTypes(webShop.ToDataScope());
                return Ok(result);
            }
            else
            {
                var result = await _masterDataService.GetMediaTypesAsync(webShop);
                return Ok(result);
            }
        }

        /// <summary>
        /// Get a collection of mediatypes for the specified WebSite (Source: KD)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/MasterData/MaterialTypes/{webShop}")]
        public async Task<IHttpActionResult> GetMaterialTypes(WebShop webShop)
        {
            if (!_isShadowMode)
            {
                var result = _masterDataFactory.GetMaterialTypes(webShop.ToDataScope());
                return Ok(result);
            }
            else
            {
                var result = await _masterDataService.GetMaterialTypesAsync(webShop);
                return Ok(result);
            }
        }

        /// <summary>
        ///  Get a collection of Areas for the specified WebSite (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/MasterData/Areas/{webShop}")]
        public async Task<IHttpActionResult> GetAreas(WebShop webShop)
        {
            if (!_isShadowMode)
            {
                var result = _masterDataFactory.GetAreas(webShop.ToDataScope());
                return Ok(result);
            }
            else
            {
                var result = await _masterDataService.GetAreasAsync(webShop);
                return Ok(result);
            }
        }

        /// <summary>
        ///  Get a collection of Subjects for the specified WebSite and for Specific Area (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/MasterData/Subjects/{webShop}/{areaId?}")]
        public async Task<IHttpActionResult> GetSubjects(WebShop webShop, int areaId = 0)
        {
            if (!_isShadowMode)
            {
                var result = _masterDataFactory.GetSubjects(webShop.ToDataScope(), areaId);
                return Ok(result);
            }
            else
            { 
                var result = await _masterDataService.GetSubjectsAsync(webShop, areaId);
                return Ok(result); 
            }
        }

        /// <summary>
        ///  Get a collection of SubAreasfor the specified WebSite and for Subject (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/MasterData/SubAreas/{webShop}/{subjectId?}")]
        public async Task<IHttpActionResult> GetSubAreas(WebShop webShop, int subjectId = 0)
        {
            if (!_isShadowMode)
            {
                var result = _masterDataFactory.GetSubAreas(webShop.ToDataScope(), subjectId);
                return Ok(result);
            }
            else
            { 
                var result = await _masterDataService.GetSubAreasAsync(webShop, subjectId);
                return Ok(result); 
            }
        }

        /// <summary>
        ///  Get a collection of Levels for the specified WebSite and for Specific Area (Source: KD)
        /// </summary>
        /// <param name="webShop"></param>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/MasterData/Levels/{webShop}/{areaId?}")]
        public async Task<IHttpActionResult> GetLevels(WebShop webShop, int areaId = 0)
        {
            if (!_isShadowMode)
            {
                var result = _masterDataFactory.GetLevels(webShop.ToDataScope(), areaId);
                return Ok(result);
            }
            else
            { 
                var result = await _masterDataService.GetLevelsAsync(webShop, areaId);
                return Ok(result); 
            }
        }
    }
}