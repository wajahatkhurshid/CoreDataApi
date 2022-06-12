using Gyldendal.Api.CoreData.Contracts.Enumerations;
using Gyldendal.Api.CoreData.Contracts.Models;
using Gyldendal.Api.CoreData.Contracts.Requests;
using Gyldendal.Api.CoreData.Contracts.Response;

namespace Gyldendal.Api.CoreData.Common.RepositoriesInfrastructure.Factories
{
    public interface ISystemSeriesFactory
    {
        /// <summary>
        /// Returns list of Series
        /// </summary>
        /// <returns></returns>
        GetSeriesResponse GetSeries(DataScope dataScope, GetSeriesRequest serieRequest);

        /// <summary>
        /// Returns list of Series
        /// </summary>
        /// <returns></returns>
        Series GetSerieById(DataScope dataScope, int serieId);
    }
}
