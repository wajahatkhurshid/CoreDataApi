using System.Collections.Generic;
using Gyldendal.Api.CommonContracts;
using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.ApiClient
{
    public partial class CoreDataServiceClient
    {
        private const string CommonLookupsController = "v1/CommonLookups";

        /// <summary>
        /// Returns a list of countries.
        /// </summary>
        /// <returns></returns>
        public List<Country> GetCountriesList()
        {
            var queryString = $"{CommonLookupsController}/GetCountriesList";

            return HttpClient.GetAsync<List<Country>>(queryString);
        }
        /// <summary>
        /// Returns a country by name.
        /// </summary>
        /// <returns></returns>
        public Country GetCountrybyName(string name)
        {
            var queryString = $"{CommonLookupsController}/GetCountrybyName/{name}";

            return HttpClient.GetAsync<Country>(queryString);
        }
        /// <summary>
        /// Returns a country by code.
        /// </summary>
        /// <returns></returns>
        public Country GetCountryByCode(string code)
        {
            var queryString = $"{CommonLookupsController}/GetCountryByCode/{code}";

            return HttpClient.GetAsync<Country>(queryString);
        }

        public List<WebShop> GetDataScopeWebShops(DataScope dataScope)
        {
            var queryString = $"{CommonLookupsController}/GetDataScopeWebShops/{dataScope}";
            return HttpClient.GetAsync<List<WebShop>>(queryString);
        }

        public DataScope GetDataScopeByWebShop(WebShop webShop)
        {
            var queryString = $"{CommonLookupsController}/GetDataScopeByWebShop/{webShop}";
            return HttpClient.GetAsync<DataScope>(queryString);
        }
    }
}
