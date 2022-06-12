using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Business.ExtensionMethods;
using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Filters;

namespace Gyldendal.Api.CoreData.Controllers
{
    /// <summary>
    /// Controller for fetching list of lookup data common for wide range of audience, such as countries list, or maybe state provinces list in future.
    /// </summary>
    [IsGdprSafe(true)]
    public class CommonLookupsController : ApiController
    {
        private readonly ICommonLookupsRepository _commonLookupsRepository;

        /// <summary>
        /// Creates a new instance of CommonLookupsController.
        /// </summary>
        /// <param name="commonLookupsRepository"></param>
        public CommonLookupsController(ICommonLookupsRepository commonLookupsRepository)
        {
            _commonLookupsRepository = commonLookupsRepository;
        }

        /// <summary>
        /// Returns a list of countries (Source: Static)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(List<Country>))]
        [Route("api/v1/CommonLookups/GetCountriesList")]
        public IHttpActionResult GetCountriesList()
        {
            return Ok(_commonLookupsRepository.GetCountriesList());
        }

        /// <summary>
        /// Returns a country by name (Source: Static)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(Country))]
        [Route("api/v1/CommonLookups/GetCountrybyName/{name}")]
        public IHttpActionResult GetCountrybyName(string name)
        {
            return Ok(_commonLookupsRepository.GetCountrybyName(name));
        }

        /// <summary>
        /// Returns a country by code (Source: Static)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(Country))]
        [Route("api/v1/CommonLookups/GetCountryByCode/{code}")]
        public IHttpActionResult GetCountryByCode(string code)
        {
            return Ok(_commonLookupsRepository.GetCountryByCode(code));
        }

        /// <summary>
        /// Get list of WebShops against dataScope
        /// </summary>
        /// <param name="dataScope"></param>
        /// <returns>List of web shops related to dataScope</returns>
        [HttpGet]
        [Route("api/v1/CommonLookups/GetDataScopeWebShops/{dataScope}")]
        public IHttpActionResult GetDataScopeWebShops(DataScope dataScope)
        {
            return Ok(dataScope.ToWebShops().OrderBy(x => (int)x).ToList());
        }

        /// <summary>
        /// Get list of WebShops against dataScope
        /// </summary>
        /// <param name="webShop"></param>
        /// <returns>List of web shops related to dataScope</returns>
        [HttpGet]
        [Route("api/v1/CommonLookups/GetDataScopeByWebShop/{webShop}")]
        public IHttpActionResult GetDataScopeByWebShop(WebShop webShop)
        {
            return Ok(webShop.ToDataScope());
        }
    }
}