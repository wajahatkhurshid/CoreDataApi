using Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Repositories;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.DataAccess.StaticLookups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gyldendal.Api.CoreData.Business.Repositories.Common
{
    public class CommonLookupsRepository : ICommonLookupsRepository
    {
        public List<Country> GetCountriesList()
        {
            return DataAccess.StaticLookups.Countries.List;
        }

        public Country GetCountrybyName(string name)
        {
            return Countries.List.FirstOrDefault(r => string.Equals(r.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public Country GetCountryByCode(string code)
        {
            return Countries.List.FirstOrDefault(r => string.Equals(r.Alpha2Code.Trim(), code.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}