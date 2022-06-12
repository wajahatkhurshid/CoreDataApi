using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.APi.CoreData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CoreData.Business.Porter.Interfaces;
using Gyldendal.Api.CoreData.Common.ConfigurationManager;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Returns GGA Related Data
    /// </summary>
    public class GGAController : ApiController
    {
        private readonly IGGAProvider _ggaprovider;

        private readonly IGgaService _ggaService;

        /// <summary>
        /// Used to switch CoreData API between running against Porter(GPM) if true and standard KD if false
        /// </summary>
        private readonly bool _isShadowMode;


        /// <param name="ggaprovider"></param>
        public GGAController(IGGAProvider ggaprovider, IGgaService ggaService, IConfigurationManager configurationManager)
        {
            _ggaprovider = ggaprovider;
            _ggaService = ggaService;
            _isShadowMode = configurationManager.IsShadowMode;
        }
        /// <summary>
        /// This method returns Title with Authors or only Authors depending upon the provided Mode 
        /// </summary>
        /// <param name="criteria">Search Criteria</param>
        /// <param name="mode">Mode 1=> Title, 2 =>Author</param>
        /// <returns>Search Results</returns>
        [HttpGet]
        [Route("api/v1/GGA/Search/{criteria}/{mode}/")]
        [ResponseType(typeof(SearchDtoResponse))]
        public async Task<IHttpActionResult> Search(string criteria, int mode)
        {
            if (!_isShadowMode)
            {
                var result = _ggaprovider.Search(criteria, mode);
                return Ok(result);
            }
            else
            {
                var result = await _ggaService.Search(criteria, mode);
                return Ok(result);
            }
        }
    }
}
