using System.Collections.Generic;
using Gyldendal.Api.CoreData.Contracts.Models;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories
{
    public interface ICommonLookupsRepository
    {
        /// <summary>
        /// Returns a list of countries.
        /// </summary>
        /// <returns></returns>
        List<Country> GetCountriesList();
        Country GetCountryByCode(string code);
        Country GetCountrybyName(string name);
    }
}
